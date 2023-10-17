using EventManager.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.DataAccess
{
    internal class AggregateProvider : IModelProvider
    {
        //ISocialProvider socialProvider;
        GroupManager groupManager;
        IDBProvider dbProvider;

        public event Action OnNewsUpdate;

        public AggregateProvider(GroupManager groupManager, IDBProvider dbProvider)
        {
            this.groupManager = groupManager;
            this.dbProvider = dbProvider;

            this.groupManager.OnNewsHappen += SocialProvider_OnNewsHappen;
        }

        private void SocialProvider_OnNewsHappen(INewsProvider sender, News news)
        {
            if (this.OnNewsUpdate != null)
                this.OnNewsUpdate();
        }

        public Event GetLastPlanedEvent()
        {
            var @event = this.groupManager.GetLastPlanedEvent();
            foreach (var memberId in this.dbProvider.GetVisitorsIdForEvent(@event))
            {
                @event.AddVisitor(socialProvider.GetUserById(memberId));
            }

            return @event;
        }

        public void PostNewSign()
        {
            this.groupManager.PostNewSign();
        }

        public async Task AddVisitorToEventAsync(Person member, Event @event)
        {
           await this.dbProvider.AddVisitAsync(member, @event);
        }

        public void AddPaymentToEvent(Person member, Event @event, int amount)
        {
            this.dbProvider.AddPaymentToEvent(member, @event, amount);
        }

        //public IEnumerable<Person> GetMembers()
        //{
        //    return this.socialProvider.GetMembers();
        //}

       

        public async Task<IEnumerable<EventPayment>> GetPaymentsForEventAsync(Event @event)
        {
            SQLiteRepository<EventPayment> rep = this.dbProvider.GetRepository<EventPayment>();
            return await rep.GetWhereAsync(p => p.IdEvent == @event.Id);
        }

        public async Task RemoveVisitorAsync(Person member, Event @event)
        {
            await this.dbProvider.RemoveVisitAsync(member, @event);
        }

        public async Task RemovePaymentAsync(Person member, Event @event)
        {
            await this.dbProvider.RemovePaymentAsync(member, @event);
        }

        public string GetGroupName()
        {
            return socialProvider.GetManagedGroup().Name;
        }
    }
}
