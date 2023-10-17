using EventManager.Common;
using EventManager.Model;
using EventManager.WPFExtentions;
using GalaSoft.MvvmLight;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.ViewModel
{
    internal class EventVisitorViewModel : ViewModelBase
    {
        private const int PAYMENT_FOR_TRAIN = 150;
        private Person member;
        private Event @event;
        private IAccountingProvider accounting;
        private IVisitorsProvider vistorsProvider;
        private IClaimersProvider claimersProvider;

        public bool IsClaimed { get { return this.claimersProvider.GetClaimedPersonsForEvent(@event).Contains(this.member, new Person.Comparer()); } }
        public NotifyTaskCompletion<bool> VisitConfirmed { get; private set; }
        public NotifyTaskCompletion<string> ConfirmVisitImage { get; private set; }
        public NotifyTaskCompletion<bool> PaymentConfirmed { get; private set; }
        public NotifyTaskCompletion<string> ConfirmPaymentImage { get; private set; }


        public EventVisitorViewModel(Person member, Event @event, IVisitorsProvider vistorsProvider, IAccountingProvider accounting, IClaimersProvider claimersProvider)
        {
            this.member = member;
            this.@event = @event;
            this.accounting = accounting;
            this.vistorsProvider = vistorsProvider;
            this.claimersProvider = claimersProvider;
            this.PaymentConfirmed = new NotifyTaskCompletion<bool>(IsPaymentConfirmedAsync());
            this.ConfirmPaymentImage = new NotifyTaskCompletion<string>(IsPaymentConfirmedImageAsync());
            this.VisitConfirmed = new NotifyTaskCompletion<bool>(IsVisitConfirmedAsync());
            this.ConfirmVisitImage = new NotifyTaskCompletion<string>(IsVisitConfirmedImageAsync());
        }

        private async Task<bool> IsPaymentConfirmedAsync()
        {
            return await this.accounting.IsPaymentConfirmedForEventAsync(this.@event, this.member);
        }

        private async Task<bool> IsVisitConfirmedAsync()
        {
            return await this.vistorsProvider.IsVisitConfirmedForEventAsync(this.@event, this.member);
        }

        private async Task<string> IsPaymentConfirmedImageAsync()
        {
            if (await IsPaymentConfirmedAsync())
                return @"/Assets/money.png";
            else
                return @"/Assets/money_pale.png";
        }


        public async Task<string> IsVisitConfirmedImageAsync()
        {
            if (await IsVisitConfirmedAsync())
                return @"/Assets/button_ok.png";
            else
                return @"/Assets/button_ok_pale.png";
        }


        public Uri Photo { get { return new Uri(member.Photo); } }
        public string Name { get { return member.FirstName + " " + member.LastName; } }



        public AsyncCommand ConfirmVisit
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (await this.vistorsProvider.IsVisitConfirmedForEventAsync(this.@event, this.member))
                    {
                        await this.vistorsProvider.UnregisterVisitAsync(member, @event);
                    }
                    else
                    {
                        await this.vistorsProvider.RegisterVisitAsync(member, @event);
                    }

                    this.ConfirmVisitImage = new NotifyTaskCompletion<string>(IsVisitConfirmedImageAsync());
                    this.RaisePropertyChanged(() => this.ConfirmVisitImage);
                });
            }
        }


        public AsyncCommand ConfirmPayment
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (await this.accounting.IsPaymentConfirmedForEventAsync(this.@event, this.member))
                    {
                        await this.accounting.RemovePaymentAsync(member, @event);
                    }
                    else
                    {
                        this.accounting.AddPaymentToEvent(member, @event, PAYMENT_FOR_TRAIN);
                    }

                    this.ConfirmPaymentImage = new NotifyTaskCompletion<string>(IsPaymentConfirmedImageAsync());
                    this.RaisePropertyChanged(() => this.ConfirmPaymentImage);
                });
            }
        }



        public int FontSize
        {
            get
            {
                return 15;
                //if (this.IsClaimed)
                //    return 10;
                //else
                //    return 5;
            }
        }
    }
}
