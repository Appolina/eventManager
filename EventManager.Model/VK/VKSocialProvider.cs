using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace EventManager.Model.VK
{
    public class VKSocialProvider : ISocialProvider
    {
        private const string CAPTCHA_NEEDED_ERROR_TEXT = "Captcha needed";
        private string appId = "5090558";

        private Uri callbackUri = new Uri("https://oauth.vk.com/blank.html");
        private Uri requestUri;
        private string rootUri = "https://api.vk.com/method";
        private IList<Person> usersCache = new List<Person>();
        private DateTime startDateTimeForUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public event NewsHappen OnNewsHappen;
        public event Action<string> OnAccessTokenGetting;
        public event EventOnCaptchaNeeded OnCaptchaNeeded;

        public string AccessToken { get; set; }
        public DateTime TokenExpiry { get; private set; }

        public bool IsLoginCompleted
        {
            get
            {
                return !string.IsNullOrEmpty(AccessToken);
            }
        }

        public VKSocialProvider(DateTime lastNewsDate, string token = null)
        {
            requestUri = new Uri($"https://oauth.vk.com/authorize?client_id={appId}&display=mobile&redirect_uri={callbackUri}&scope=wall,friends,video,offline&response_type=token&v=5.40");
            this.AccessToken = token;
        }


        public async Task UpdatePostText(int postId, string text, int ownerId)
        {
            var postCommand = $"{this.rootUri}/wall.edit?owner_id={ownerId}&post_id={postId}&message={WebUtility.UrlEncode(text)}&access_token={this.AccessToken}";
            await Post(postCommand);
        }      

        public IEnumerable<Comment> GetCommentsFromPost(int postId, int ownerId)
        {
            var vkComments = this.getObjects<VKComment>("wall.getComments", $"owner_id={ownerId}&post_id={postId}&count=30");
            return vkComments.Select(vkc => new Comment(vkc.Text, vkc.FromId, getDateTimeFromUnixTime(vkc.Date)));
        }

        public IEnumerable<WallPost> GetWallPostsFromGroup(int groupId)
        {
            var vkWalls = this.getObjects<VKWallPost>("wall.get", $"owner_id=-{groupId}");
            return vkWalls.Select(vkp => new WallPost(vkp.Id, vkp.Owner, Regex.Replace(vkp.Text, @"\u00A0", " ").Replace("<br>", Environment.NewLine), getDateTimeFromUnixTime(vkp.Date)));
        }

        private DateTime getDateTimeFromUnixTime(long unixDateTime)
        {
            return startDateTimeForUnixTime.AddSeconds(unixDateTime).ToLocalTime();
        }

        public Person GetUserById(long id)
        {
            var userFromCache = usersCache.SingleOrDefault(m => m.Id == id);

            if (userFromCache == null)
            {
                var vkMember = getObject<VKMember>("users.get", $"user_ids={id}&fields=photo_100");
                var user = new Person(vkMember.Id) { FirstName = vkMember.FirstName, LastName = vkMember.LastName, Photo = vkMember.Photo };
                addUserToCache(user);
                return user;
            }
            else
            {
                return userFromCache;
            }
        }

        private void addUserToCache(Person user)
        {
            var sameUserInCache = usersCache.SingleOrDefault(u => u.Id == user.Id);

            if (usersCache != null)
            {
                usersCache.Remove(sameUserInCache);
            }

            this.usersCache.Add(user);
        }

        private TResult getObject<TResult>(string methodName, string parameters)
        {
            var responseString = getResponse($"{this.rootUri}/{methodName}?{parameters}&access_token={this.AccessToken}");

            JObject o = JObject.Parse(responseString);
            JArray array = (JArray)o["response"];

            return JsonConvert.DeserializeObject<TResult>(array[0].ToString());
        }

        private IEnumerable<TResultItem> getObjects<TResultItem>(string methodName, string parameters, bool withAccessToken = true)
        {
            var requestString = $"{this.rootUri}/{methodName}?{parameters}";
            if (withAccessToken)
                requestString += $"&access_token={this.AccessToken}";

            var responseString = getResponse(requestString);

            JObject o = JObject.Parse(responseString);
            JArray array = (JArray)o["response"];

            var first = true;

            foreach (var it in array)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return JsonConvert.DeserializeObject<TResultItem>(it.ToString());
                }
            }
        }

        public bool CompleteAuth(WebAuthenticationBrokerContinuationEventArgs args)
        {
            var result = args.WebAuthenticationResult;

            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.ErrorHttp:
                    return false;
                case WebAuthenticationStatus.Success:
                    var pattern = string.Format("{0}#access_token={1}&expires_in={2}", callbackUri, "(?<access_token>.+)", "(?<expires_in>.+)");
                    var match = Regex.Match(result.ResponseData, pattern);

                    var access_token = match.Groups["access_token"];
                    var expires_in = match.Groups["expires_in"];

                    AccessToken = access_token.Value;

                    if (OnAccessTokenGetting != null)
                        OnAccessTokenGetting(AccessToken);
                    //TokenExpiry = DateTime.Now.AddSeconds(double.Parse(expires_in.Value));

                    return true;
                case WebAuthenticationStatus.UserCancel:
                    return false;
                default:
                    return false;
            }
        }

        public void RequestLogin()
        {
            WebAuthenticationBroker.AuthenticateAndContinue(requestUri,
                                                 callbackUri,
                                                 null,
                                                  WebAuthenticationOptions.None);
        }

        public async Task PostText(int ownerId, string text)
        {
            var postCommand = $"{this.rootUri}/wall.post?owner_id=-{ownerId}&from_group=1&message={WebUtility.UrlEncode(text)}&lang=en";
            await Post(postCommand);
        }


        public async Task Post(string command, string captchaFields = null)
        {
            using (var client = new HttpClient())
            {
                var resCommand = command;
                if (!string.IsNullOrEmpty(captchaFields))
                    resCommand += $"&{captchaFields}";

                resCommand += $"&access_token={this.AccessToken}";

                var response = await client.PostAsync(resCommand, null);
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains(CAPTCHA_NEEDED_ERROR_TEXT))
                {
                    JObject o = JObject.Parse(content);
                    JObject errorString = (JObject)o["error"];

                    var error = JsonConvert.DeserializeObject<CaptchaNeddedError>(errorString.ToString());

                    if (OnCaptchaNeeded != null)
                    {
                        var captcha_key = await OnCaptchaNeeded(error.CaptchaSid, error.CaptchaImg);

                        if (!string.IsNullOrEmpty(captcha_key))
                        {
                            await Post(command, $"captcha_sid={error.CaptchaSid}&captcha_key={WebUtility.UrlEncode(captcha_key)}");
                        }
                    }
                }
            }
        }



        private string getResponse(string request)
        {
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var task = client.GetAsync(request);

                    using (var response = task.Result)
                    {
                        var task1 = response.Content.ReadAsStringAsync();
                        var result = task1.Result;
                        if (result.IndexOf("Too many requests per second") < 0)
                            return result;
                    }
                    Task.Delay(300).Wait();
                }
            }
        }

        public Group GetGroupById(int groupId)
        {
            var vkGroup = getObject<VKGroup>("groups.getById", $"group_ids={groupId}&fields=photo_100");
            return new Group(vkGroup.Id, this) { Name = vkGroup.Name, PhotoUri = vkGroup.PhotoUri };
        }

        public IEnumerable<Group> GetModeratedGroups()
        {
            return this.getObjects<VKGroup>("groups.get", "filter=moder&extended=1")
                .Select(vg => new Group(vg.Id, this) { PhotoUri = vg.PhotoUri, Name = vg.Name });
        }

        public IEnumerable<Person> GetGroupMembers(int id)
        {
            var methodName = "groups.getMembers";
            var parameters = $"group_id={id}&fields=name,photo_100";

            var responseString = getResponse($"{this.rootUri}/{methodName}?{parameters}&access_token={this.AccessToken}");
            JObject o = JObject.Parse(responseString);
            JArray array = (JArray)o["response"]["users"];

            var first = true;

            foreach (var it in array)
            {
                if (first)
                {
                    first = false;
                }
                //else
                //{
                var vkm = JsonConvert.DeserializeObject<VKMember>(it.ToString());
                var member = new Person(vkm.Id) { FirstName = vkm.FirstName, LastName = vkm.LastName, Photo = vkm.Photo };

                this.addUserToCache(member);

                yield return member;
                //}
            }
        }

        public WallPost GetWallPostById(int idPost)
        {
            var vkWalls = this.getObjects<VKWallPost>("wall.getById", $"posts = {idPost}");
            var post = vkWalls.First();
            return new WallPost(post.Id, post.Owner, Regex.Replace(post.Text, @"\u00A0", " ").Replace("<br>", Environment.NewLine), getDateTimeFromUnixTime(post.Date));

        }
    }
}

