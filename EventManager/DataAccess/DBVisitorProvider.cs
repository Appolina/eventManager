using EventManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManager.Model;

namespace EventManager.DataAccess
{
    internal class DBVisitorProvider : IVisitorsProvider
    {
        private IDBProvider dbProvider;

        public DBVisitorProvider(IDBProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        public async Task<bool> IsVisitConfirmedForEventAsync(Event @event, Person member)
        {
            var visits = await this.GetVisitsForEventAsync(@event);
            return visits.Any(p => p.IdPlayer == member.Id);
        }

        public async Task<IEnumerable<EventVisit>> GetVisitsForEventAsync(Event @event)
        {
            var rep = this.dbProvider.GetRepository<EventVisit>();
            return await rep.GetWhereAsync(p => p.IdEvent == @event.Id);
        }


        public async Task UnregisterVisitAsync(Person member, Event @event)
        {
            var rep = this.dbProvider.GetRepository<EventVisit>();
            var objectToDelete = await rep.GetWhereAsync(ev => ev.IdEvent == @event.Id && ev.IdPlayer == member.Id);
            await rep.DeleteAsync(objectToDelete.First());
        }


        public async Task RegisterVisitAsync(Person member, Event @event)
        {
            await this.dbProvider.GetRepository<EventVisit>().AddAsync(new EventVisit() { IdEvent = @event.Id, IdPlayer = member.Id });
        }
        
    }
}
