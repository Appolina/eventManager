using EventManager.Model;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace EventManager.ViewModel
{
    internal class EventVisitorsViewModel : ViewModelBase
    {
        private Event @event;
        private Group group;
        private IAccountingProvider accounting;
        private IVisitorsProvider visitorsProvider;
        private IClaimersProvider claimersProvider;

        public EventVisitorsViewModel(Group group, Event @event, IAccountingProvider accounting, IVisitorsProvider visitorsProvider, IClaimersProvider claimersProvider)
        {
            this.@event = @event;
            this.group = group;
            this.accounting = accounting;
            this.visitorsProvider = visitorsProvider;
            this.claimersProvider = claimersProvider;
        }

        public IEnumerable<EventVisitorViewModel> ClaimedMembers
        {
            get
            {
                IEnumerable<Person> members = this.group.Members;
                var membersVM = members
                                    .Select(m =>
                                        new EventVisitorViewModel(m, this.@event, visitorsProvider, this.accounting, this.claimersProvider));

                return membersVM.OrderByDescending(m => m.IsClaimed);
            }
        }

        public string GroupName
        {
            get { return this.group.Name; }
        }

    }
}
