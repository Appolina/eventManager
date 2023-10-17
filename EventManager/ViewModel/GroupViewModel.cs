using EventManager.Model;
using System;

namespace EventManager.ViewModel
{
    class GroupViewModel
    {
        public Group Group { get; private set; }

        public GroupViewModel(Group group)
        {
            this.Group = group;
        }

        public GroupViewModel()
        {
        }

        public string Name
        {
            get
            {
                if (this.Group == null)
                    return "Выберите группу";
                else
                    return this.Group.Name;
            }
        }
        public Uri Photo
        {
            get
            {
                if (this.Group == null)
                    return null;
                else
                    return new Uri(Group.PhotoUri);
            }
        }
    }
}
