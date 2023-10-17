using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManager.Model
{
    public interface IEventPaymentProvider
    {
        Task<IEnumerable<EventPayment>> PaymentsForEventAsync(Event @event);
    }
}