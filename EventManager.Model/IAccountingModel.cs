using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.Model
{
    internal interface IAccountingModel
    {
        Task<IEnumerable<EventPayment>> GetPaymentsForEventAsync(Event @event);
        Task RemovePaymentAsync(Person member, Event @event);
        void AddPaymentToEvent(Person member, Event @event, int amount);
    }
}