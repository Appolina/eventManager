using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Model
{
    public class GroupManager : INewsMaker
    {
        private Group group;
        private ISocialProvider socialProvider;
        private DateTime lastNewsDate;
        private WallForEventParser wallPostParser;
        private IEnumerable<IEvent> events;
        private bool updatePostWithClaimersNeeded = false;

        public event NewsHappen OnNewsHappen;

        public IEvent ActualEvent
        {
            get { return this.Events.First(); }
        }


        public Group Group
        {
            set
            {
                this.group = value;
            }
        }

        private IEnumerable<IEvent> Events
        {
            get
            {
                if (this.events == null)
                {
                    fillEvents();
                }
                return events;
            }
        }

        private void fillEvents()
        {
            events = getEventsFromSocial();
        }

        private IEnumerable<IEvent> getEventsFromSocial()
        {
            var posts = this.socialProvider.GetWallPostsFromGroup(this.group.Id).Where(p => wallPostParser.IsEventPost(p));

            if (posts.Any())
            {
                foreach (var post in posts)
                {
                    if (this.lastNewsDate < post.Date)
                    {
                        if (this.OnNewsHappen != null)
                            this.OnNewsHappen(this, new News(post.Date, this.group.PhotoUri, News.enNewsType.EventDeclare));
                    }

                    yield return wallPostParser.GetEventFromPost(post);
                }
            }
            else
            {
                yield return new NullEvent();
            }

        }

        public GroupManager(Group group, ISocialProvider socialProvider, DateTime lastNewsDate, string signSuffix = null)
        {
            this.group = group;
            this.socialProvider = socialProvider;
            this.lastNewsDate = lastNewsDate;

            wallPostParser = new WallForEventParser(signSuffix);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    //this.fillEvents();
                    if (this.ActualEvent is Event)
                        GetClaimedPersonsForEvent((Event)this.ActualEvent).ToArray();

                    if (updatePostWithClaimersNeeded)
                    {
                        updatePost();
                        updatePostWithClaimersNeeded = false;
                    }
                    await Task.Delay(5000);
                }
            });
        }

        private void updatePost()
        {
            var @event = (Event)this.ActualEvent;
            var text = wallPostParser.UpdateTextWithClaimers(this.socialProvider.GetWallPostById(@event.IdPost), this.GetClaimedPersonsForEvent(@event));
            this.socialProvider.UpdatePostText(@event.IdPost, text, -this.group.Id);
        }


        public async Task PostNewSign()
        {
            var textForSign = wallPostParser.TextForNewEventSign();
            await this.socialProvider.PostText(this.group.Id, textForSign);
        }


        public IEvent GetLastPlanedEvent()
        {
            return this.ActualEvent;
        }

        private void createNews(DateTime date, Person member, string text)
        {
            this.lastNewsDate = date;

            if (this.OnNewsHappen != null)
                this.OnNewsHappen(this, new News(date, member.Photo, News.enNewsType.MemerClaimed, text));
        }

        public IEnumerable<Person> GetClaimedPersonsForEvent(Event @event)
        {
            foreach (var comment in this.socialProvider.GetCommentsFromPost(@event.IdPost, -this.group.Id))
            {
                var commentParser = new CommentForEventParser(this.socialProvider.GetGroupMembers(this.group.Id));

                var parseResult = commentParser.ParseForClaim(comment);

                if (parseResult.Success)
                {
                    var member = this.socialProvider.GetUserById(parseResult.PersonId);

                    yield return member;

                    if (comment.Date > this.lastNewsDate)
                    {
                        var text = $"{member.FirstName} {member.LastName} записался на занятие {@event.EventDate.ToString("d MMMM")}";
                        createNews(comment.Date, member, text);

                        this.updatePostWithClaimersNeeded = true;
                    }
                }
            }
        }

        public string GetGroupName()
        {
            return this.group.Name;
        }
    }
}
