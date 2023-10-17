using System.Collections.Generic;

namespace EventManager.Model
{
    public interface IClaimersProvider
    {
        IEnumerable<Person> GetClaimedPersonsForEvent(Event @event);
    }
}