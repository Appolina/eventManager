using EventManager.Model;
using EventManager.WPFExtentions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.ViewModel
{
    internal class WelcomeViewModel : ViewModelBase
    {
        private GroupManager groupManager;
        private INewsProvider newsProvider;
        private IEvent @event;
        public event Action OnCheckVisitorsNavigate;
        private INavigationService navigationService;
        private bool showCaptchaPopup = false;
        private IAccountingProvider accountingProvider;
        private ClaimerProviderFromGroupManager claimedPersonsProvider;

        public event Action<string> OnCaptchaEntered;

        public WelcomeViewModel(GroupManager groupManager, INavigationService navigationService, INewsProvider newsProvider, IAccountingProvider accountingProvider)
        {
            this.groupManager = groupManager;
            this.navigationService = navigationService;
            this.accountingProvider = accountingProvider;
            this.claimedPersonsProvider = new ClaimerProviderFromGroupManager(this.groupManager);
            this.newsProvider = newsProvider;
            this.newsProvider.OnNewsUpdate += ModelProvider_OnNewsUpdate;
        }

        private void ModelProvider_OnNewsUpdate()
        {
            this.RaisePropertyChanged(() => this.News);
        }

        private IEvent Event
        {
            get
            {
                if (this.@event == null)
                {
                    var task = groupManager.GetLastPlanedEvent();
                    //task.Wait();
                    @event = task;
                }
                return @event;
            }
        }

        public string Decription
        {
            get
            {
                if (Event.IsNull || !Event.IsPlannedEvent)
                    return "Занятий не запланировано";
                else
                    return $"Следующее занятие состоится {Event.EventDate.ToString("d MMMM")}";
            }
        }


        public string EventInfo
        {
            get
            {
                if (this.@event is Event)
                    return $"Записано {claimedPersonsProvider.GetClaimedPersonsForEvent((Event)this.@event).Count()} человек";
                else
                    return string.Empty;
            }
        }

        public bool IsPlannedEvent { get { return this.Event.IsPlannedEvent; } }

        public bool NoPlannedEvent { get { return !this.Event.IsPlannedEvent; } }


        public AsyncCommand PostSign
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    await this.groupManager.PostNewSign();
                    this.@event = null;
                    this.RaisePropertyChanged(() => this.News);
                    this.RaisePropertyChanged(() => this.Decription);
                    this.RaisePropertyChanged(() => this.EventInfo);
                    this.RaisePropertyChanged(() => this.IsPlannedEvent);
                    this.RaisePropertyChanged(() => this.NoPlannedEvent);
                });
            }
        }

        public RelayCommand CheckVisit { get { return new RelayCommand((o) => navigationService.NavigateTo("/View/EventVisitors.xaml"), (o) => true); } }

        public IEnumerable<NewsViewModel> News
        {
            get { return this.newsProvider.GetNews().OrderByDescending(n => n.NewsDate).Take(15).Select(n => new NewsViewModel(n)); }
        }

        public string GroupName
        {
            get { return this.groupManager.GetGroupName(); }
        }

        public bool ShowCaptchaPopup
        {
            get { return showCaptchaPopup; }
        }

        public Uri CaptchaImageUrl
        {
            get; private set;
        }

        public bool HubSectionEnabled
        {
            get { return !showCaptchaPopup; }
        }

        public RelayCommand Ok
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    showCaptchaPopup = false;
                    this.RaisePropertyChanged(() => this.ShowCaptchaPopup);
                }, (o) => true);
            }
        }

        public string CapthaText { get; set; }

        internal async Task<string> OnCaptchaNeeded(string captchaSid, string captchaImg)
        {
            this.CaptchaImageUrl = new Uri(captchaImg);
            this.showCaptchaPopup = true;
            this.RaisePropertyChanged(() => this.ShowCaptchaPopup);
            this.RaisePropertyChanged(() => this.CaptchaImageUrl);

            return await Task.Run
                (async () =>
                {
                    while (showCaptchaPopup)
                    {
                        await Task.Delay(100);
                    }

                    return this.CapthaText;
                }
            );
        }

        public TitleAccountingViewModel TitleAccountingViewModel
        {
            get { return new TitleAccountingViewModel(this.accountingProvider, this.navigationService); }
        }
    }

}
