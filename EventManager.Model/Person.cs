using System.Collections.Generic;

namespace EventManager.Model
{
    public class Person
    {
        public int Id { get; private set; }

        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string Photo { get; internal set; }

        public Person(int id)
        {
            this.Id = id;
        }

        public class Comparer : IEqualityComparer<Person>
        {
            public bool Equals(Person x, Person y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Person codeh)
            {
                return codeh.Id.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Id})";
        }
    }
}
