using System.Threading.Tasks;
using EventManager.Model;

namespace EventManager.ViewModel
{
    internal interface IVisitorsProvider
    {
        Task RegisterVisitAsync(Person member, Event @event);
        Task<bool> IsVisitConfirmedForEventAsync(Event @event, Person member);
        Task UnregisterVisitAsync(Person member, Event @event);
    }
}