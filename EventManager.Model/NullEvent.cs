using System;
using System.Collections.Generic;

namespace EventManager.Model
{
    internal class NullEvent : IEvent
    {
        public NullEvent()
        {
        }

        public IEnumerable<Person> ClaimedMembers
        {
            get
            {
                return new List<Person>();
            }

        }

        public DateTime EventDate
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        

        public bool IsNull
        {
            get
            {
                return true;
            }

        }

        public bool IsPlannedEvent
        {
            get
            {
                return false;
            }
        }
    }
}