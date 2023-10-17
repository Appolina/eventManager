using System;
using EventManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Windows.ApplicationModel.Activation;

namespace EventManager.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        private IAuthicatiable authicatiable;
        private INavigationService navigationService;

        public event Action OnNavigateForward;

        public LoginViewModel(IAuthicatiable socialProvider, INavigationService navigationService)
        {
            this.authicatiable = socialProvider;
            this.navigationService = navigationService;            
        }
        
        public bool IsLoginCompleted { get { return authicatiable.IsLoginCompleted; } }

        public RelayCommand Login
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (authicatiable.IsLoginCompleted)
                    {
                        this.navigationService.NavigateTo("/View/MainView.xaml");
                    }
                    else
                    {
                        authicatiable.RequestLogin();
                    }
                }
                , (o) => true);
            }
        }

        internal bool CompleteAuth(WebAuthenticationBrokerContinuationEventArgs args)
        {
            return authicatiable.CompleteAuth(args);
        }

        internal void NavigateForwad()
        {
            if (OnNavigateForward != null)
                OnNavigateForward();
        }

    }
}
