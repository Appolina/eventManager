/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:EventManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using EventManager.Model;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Views;
using EventManager.DataAccess;

namespace EventManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the
    ///  bindings.
    /// </summary>
    public class ViewModelLocator 
    {
        private ISocialProvider socialProvider = SimpleIoc.Default.GetInstance<ISocialProvider>();
        private IAuthicatiable authicatiable = SimpleIoc.Default.GetInstance<IAuthicatiable>();
        private Settings settingsModel;
        private GroupManager groupManager;
        private WelcomeViewModel welcomeViewModel;
        private Accounting accounting;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var groupIdValue = App.repo.GetGroupIdValue();
            int? groupId = null;

            if (!string.IsNullOrEmpty(groupIdValue))
                groupId = int.Parse(groupIdValue);

            settingsModel = new Settings(groupId, App.repo.GetSignSuffixValue());
            settingsModel.OnSettingValueChanged += settings_OnSettingValueChanged;


            accounting = new Accounting(App.repo);

        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public object LoginViewModel
        {
            get
            {
                var loginViewModel = new LoginViewModel(authicatiable, SimpleIoc.Default.GetInstance<INavigationService>());
                loginViewModel.OnNavigateForward += LoginViewModel_OnNavigateForward;
                return loginViewModel;
            }
        }

        private void LoginViewModel_OnNavigateForward()
        {
            if (this.settingsModel.GroupId.HasValue && !string.IsNullOrEmpty(this.settingsModel.SignSuffix))
            {
                SimpleIoc.Default.GetInstance<INavigationService>().NavigateTo("/View/MainView.xaml");
            }
            else
            {
                SimpleIoc.Default.GetInstance<INavigationService>().NavigateTo("/View/SettingsView.xaml");
            }
        }



        public object EventVisitorsViewModel
        {
            get
            {
                var group = this.socialProvider.GetGroupById(int.Parse(App.repo.GetGroupIdValue()));
                var @event = (Event)groupManager.GetLastPlanedEvent();

                var eventVisitorsViewModel = new EventVisitorsViewModel(group, @event, accounting, new DBVisitorProvider(App.repo), new ClaimerProviderFromGroupManager(this.groupManager));
                return eventVisitorsViewModel;
            }
        }


        public object SettingsViewModel
        {
            get
            {

                return new SettingsViewModel(settingsModel, this.socialProvider, SimpleIoc.Default.GetInstance<INavigationService>());
            }
        }

        public object WelcomeViewModel
        {
            get
            {
                var group = this.socialProvider.GetGroupById(int.Parse(App.repo.GetGroupIdValue()));
                // todo do we need to create a groupManager every time?
                groupManager = new GroupManager(group, this.socialProvider, App.repo.GetLatestNewsDate(), App.repo.GetSignSuffixValue());
                App.repo.AddNewsProvider(groupManager);
                if (welcomeViewModel != null)
                    this.socialProvider.OnCaptchaNeeded -= welcomeViewModel.OnCaptchaNeeded;

                welcomeViewModel = new WelcomeViewModel(groupManager, SimpleIoc.Default.GetInstance<INavigationService>(), App.repo, accounting);

                this.socialProvider.OnCaptchaNeeded += welcomeViewModel.OnCaptchaNeeded;
                return welcomeViewModel;
            }
        }


        private void settings_OnSettingValueChanged(string settingName, object settingValue)
        {
            var stringSttingValue = settingValue == null ? string.Empty : settingValue.ToString();
            App.repo.SaveSettingValue(settingName, stringSttingValue);
        }
    }
}