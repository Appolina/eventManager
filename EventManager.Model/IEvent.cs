using System;

namespace EventManager.Model
{
    public interface IEvent
    {
        DateTime EventDate { get; }
        bool IsNull { get; }
        bool IsPlannedEvent { get; }
    }
}