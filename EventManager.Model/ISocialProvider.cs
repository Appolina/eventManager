using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.Model
{
    public delegate Task<string> EventOnCaptchaNeeded(string captchaSid, string captchaImg);

    public interface ISocialProvider : INewsMaker, IAuthicatiable
    {
        Person GetUserById(long id);
        event Action<string> OnAccessTokenGetting;

        Group GetGroupById(int groupId);
        IEnumerable<Person> GetGroupMembers(int id);
        IEnumerable<Group> GetModeratedGroups();
        Task PostText(int ownerId, string text);
        IEnumerable<WallPost> GetWallPostsFromGroup(int groupId);
        IEnumerable<Comment> GetCommentsFromPost(int postId, int ownerId);
        Task UpdatePostText(int postId, string text, int ownerId);
        event EventOnCaptchaNeeded OnCaptchaNeeded;
        WallPost GetWallPostById(int idPost);
    }
}