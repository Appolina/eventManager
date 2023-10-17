using System;
using System.Collections.Generic;

namespace EventManager.Model
{
    public class Group
    {
        public int Id { get; private set; }
        public string PhotoUri { get; internal set; }
        public string Name { get; internal set; }

        public IEnumerable<Person> Members
        {
            get
            {
                if (members == null)
                    members = socialProvider.GetGroupMembers(this.Id);

                return members;
            }
        }

        private ISocialProvider socialProvider;
        private IEnumerable<Person> members;

        public Group(int id, ISocialProvider socialProvider)
        {
            Id = id;
            this.socialProvider = socialProvider;
        }
    }
}
