using EventManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EventManager.DataAccess
{
    internal class Accounting : IAccountingProvider
    {
        private IDBProvider dbProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public int BallFundAmount
        {
            get
            {
                var rep = this.dbProvider.GetRepository<AccountOperation>();
                //var op = await rep.GetAllAsync();
                return 5;
            }
        }

        public Accounting(IDBProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        public async Task<IEnumerable<EventPayment>> GetPaymentsForEventAsync(Event @event)
        {
            var rep = this.dbProvider.GetRepository<EventPayment>();
            return await rep.GetWhereAsync(p => p.IdEvent == @event.Id);
        }

        public async Task<bool> IsPaymentConfirmedForEventAsync(Event @event, Person member)
        {
            var payments = await this.GetPaymentsForEventAsync(@event);
            return payments.Any(p => p.IdPayer == member.Id);
        }

        public async Task RemovePaymentAsync(Person member, Event @event)
        {
            await this.dbProvider.RemovePaymentAsync(member, @event);
        }

        public void AddPaymentToEvent(Person member, Event @event, int amount)
        {
            this.dbProvider.AddPaymentToEvent(member, @event, amount);
        }

        public Task AddRentPaymentAsync()
        {
            throw new NotImplementedException();
        }
    }
}
