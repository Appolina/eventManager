using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace EventManager.Model
{
    public class SocialProviderWithCache : ISocialProvider
    {
        private ISocialProvider cachedSocialProvider;

        // cache staff
        private Dictionary<long, Person> persons = new Dictionary<long, Person>();
        private Dictionary<int, IEnumerable<Person>> groupMembersDictionary = new Dictionary<int, IEnumerable<Person>>();
        private IEnumerable<Group> moderatedGroups;
        private Dictionary<int, IEnumerable<WallPost>> groupPostsDictionary = new Dictionary<int, IEnumerable<WallPost>>();
        private Dictionary<int, WallPost> postsDictionary = new Dictionary<int, WallPost>();
        private Dictionary<int, IEnumerable<Comment>> postCommentsDictionary = new Dictionary<int, IEnumerable<Comment>>();

        public event Action<string> OnAccessTokenGetting;
        public event EventOnCaptchaNeeded OnCaptchaNeeded;
        public event NewsHappen OnNewsHappen;

        public SocialProviderWithCache(ISocialProvider cachedSocialProvider)
        {
            this.cachedSocialProvider = cachedSocialProvider;
            this.cachedSocialProvider.OnAccessTokenGetting += CachedSocialProvider_OnAccessTokenGetting;
            this.cachedSocialProvider.OnCaptchaNeeded += CachedSocialProvider_OnCaptchaNeeded;
            this.cachedSocialProvider.OnNewsHappen += CachedSocialProvider_OnNewsHappen;
        }

        
        public IEnumerable<Comment> GetCommentsFromPost(int postId, int ownerId)
        {
            if (!postCommentsDictionary.ContainsKey(postId))
                postCommentsDictionary[postId] = this.cachedSocialProvider.GetCommentsFromPost(postId, ownerId);

            return postCommentsDictionary[postId];
        }

        public Group GetGroupById(int groupId)
        {
            if (this.moderatedGroups == null)
                this.moderatedGroups = this.cachedSocialProvider.GetModeratedGroups();

            return this.moderatedGroups.Single(gr => gr.Id == groupId);
        }


        public IEnumerable<Person> GetGroupMembers(int id)
        {
            if (!this.groupMembersDictionary.ContainsKey(id))
            {
                this.groupMembersDictionary[id] = this.cachedSocialProvider.GetGroupMembers(id);
            }

            return this.groupMembersDictionary[id];
        }

        public IEnumerable<Group> GetModeratedGroups()
        {
            if (this.moderatedGroups == null)
                this.moderatedGroups = this.cachedSocialProvider.GetModeratedGroups();

            return this.moderatedGroups;

        }

        public Person GetUserById(long id)
        {
            if (!persons.ContainsKey(id))
            {
                persons[id] = this.cachedSocialProvider.GetUserById(id);
            }
            return persons[id];
        }

        public WallPost GetWallPostById(int idPost)
        {
            var cachedPosts = this.groupPostsDictionary.Values.SelectMany(v => v);

            if (cachedPosts.Any(wp => wp.Id == idPost))
            {
                return cachedPosts.First(wp => wp.Id == idPost);
            }
            else
            {
                if (!postsDictionary.ContainsKey(idPost))
                {
                    postsDictionary[idPost] = this.cachedSocialProvider.GetWallPostById(idPost);
                }
                return postsDictionary[idPost];
            }

        }

        public IEnumerable<WallPost> GetWallPostsFromGroup(int groupId)
        {
            if (!groupPostsDictionary.ContainsKey(groupId))
            {
                this.groupPostsDictionary[groupId] = this.cachedSocialProvider.GetWallPostsFromGroup(groupId);
            }

            return groupPostsDictionary[groupId];
        }

        #region Just usual wrapping

        public Task PostText(int ownerId, string text)
        {
            return this.cachedSocialProvider.PostText(ownerId, text);
        }

        public void RequestLogin()
        {
            this.cachedSocialProvider.RequestLogin();
        }

        public Task UpdatePostText(int postId, string text, int ownerId)
        {
            return this.cachedSocialProvider.UpdatePostText(postId, text, ownerId);
        }

        private void CachedSocialProvider_OnNewsHappen(INewsMaker sender, News news)
        {
            if (this.OnNewsHappen != null)
                this.OnNewsHappen(sender, news);
        }

        private Task<string> CachedSocialProvider_OnCaptchaNeeded(string captchaSid, string captchaImg)
        {
            if (this.OnCaptchaNeeded != null)
                return this.OnCaptchaNeeded(captchaSid, captchaImg);
            else return null;
        }

        private void CachedSocialProvider_OnAccessTokenGetting(string obj)
        {
            if (this.OnAccessTokenGetting != null)
                this.OnAccessTokenGetting(obj);
        }

        public bool IsLoginCompleted
        {
            get
            {
                return this.cachedSocialProvider.IsLoginCompleted;
            }
        }

        public bool CompleteAuth(WebAuthenticationBrokerContinuationEventArgs args)
        {
            return this.cachedSocialProvider.CompleteAuth(args);
        }

        #endregion
    }
}
