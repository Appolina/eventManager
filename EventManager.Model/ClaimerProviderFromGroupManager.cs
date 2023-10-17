using System.Collections.Generic;

namespace EventManager.Model
{
    public class ClaimerProviderFromGroupManager : IClaimersProvider
    {
        private GroupManager groupManager;
        private Dictionary<Event, IEnumerable<Person>> cache = new Dictionary<Event, IEnumerable<Person>>();

        public ClaimerProviderFromGroupManager(GroupManager groupManager)
        {
            this.groupManager = groupManager;
            this.groupManager.OnNewsHappen += GroupManager_OnNewsHappen;
        }

        public IEnumerable<Person> GetClaimedPersonsForEvent(Event @event)
        {
            if (!cache.ContainsKey(@event))
                cache[@event] = this.groupManager.GetClaimedPersonsForEvent(@event);


            return this.cache[@event];
        }

        private void GroupManager_OnNewsHappen(INewsMaker sender, News news)
        {
            cache.Clear();
        }
    }
}
