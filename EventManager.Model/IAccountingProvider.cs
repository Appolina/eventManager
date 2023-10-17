using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EventManager.Model
{
    public interface IAccountingProvider : INotifyPropertyChanged
    {
        int BallFundAmount { get;  }

        Task<IEnumerable<EventPayment>> GetPaymentsForEventAsync(Event @event);

        Task<bool> IsPaymentConfirmedForEventAsync(Event @event, Person member);

        Task RemovePaymentAsync(Person member, Event @event);

        void AddPaymentToEvent(Person member, Event @event, int amount);

        Task AddRentPaymentAsync();
    }
}