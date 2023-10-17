using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventManager.Model
{
    public class Event: IEvent
    {   
        private ICollection<Person> claimedMembers = new List<Person>();

        public int Id { get; private set; }

        public int IdPost { get; private set; }

      
        public DateTime EventDate { get; set; }
                
        internal Event(DateTime date, int idPost) 
        {
            this.EventDate = date;
            this.IdPost = idPost;
            this.Id = idPost;
        }
        
        //public IEnumerable<Person> ClaimedMembers { get { return this.claimedMembers; } }
        
        public bool IsPlannedEvent { get { return this.EventDate > DateTime.Now.AddDays(-2); } }
        
        public bool IsNull { get { return false; } }

        internal void AddClaimedMember(Person member)
        {
            this.claimedMembers.Add(member);
        }
        
    }
}
