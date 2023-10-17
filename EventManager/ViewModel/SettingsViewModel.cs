using EventManager.Model;
using GalaSoft.MvvmLight.Views;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EventManager.ViewModel
{
    class SettingsViewModel : ViewModelBaseWithValidation

    {
        private ISettingsModel settingsProvider;
        private ISocialProvider socialProvider;
        private INavigationService navigationService;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public SettingsViewModel(ISettingsModel settingsProvider, ISocialProvider socialProvider, INavigationService navigationService)
        {
            this.settingsProvider = settingsProvider;
            this.socialProvider = socialProvider;
            this.navigationService = navigationService;
        }

        public IEnumerable<GroupViewModel> Groups
        {
            get
            {
                return this.socialProvider.GetModeratedGroups().Select(g => new GroupViewModel(g));
            }
        }

        public GroupViewModel SelectedGroup
        {
            get
            {
                if (settingsProvider.GroupId.HasValue)
                    return new GroupViewModel(this.socialProvider.GetModeratedGroups().Single(g => g.Id == settingsProvider.GroupId.Value));
                else
                    return new GroupViewModel();
            }
            set
            {
                settingsProvider.GroupId = value.Group.Id;
                this.RaisePropertyChanged(() => this.Next);
            }
        }

        public string SignSuffix
        {
            get { return this.settingsProvider.SignSuffix; }
            set { this.settingsProvider.SignSuffix = value; }
        }

        public string GroupName
        {
            get
            {
                if (settingsProvider.GroupId.HasValue)
                    return this.socialProvider.GetModeratedGroups().Single(g => g.Id == settingsProvider.GroupId.Value).Name;
                else
                    return string.Empty;
            }
        }

        public RelayCommand Next
        {
            get
            {
                return new RelayCommand(
                    (o) => this.navigationService.NavigateTo("/View/MainView.xaml"),
                    (o) =>
                    {
                        this.Validate();
                        return this.IsValid;
                    });
            }
        }

        public bool TextValidationTextVisibility
        {
            get;
            private set;
        } = false;

        public string TextValidationText
        {
            get { return this.ValidationErrors["SignSuffix"]; }
        }


        protected override void ValidateSelf()
        {
            this.ValidationErrors.Clear();
            this.TextValidationTextVisibility = true;

            if (string.IsNullOrEmpty(SignSuffix))
            {
                this.ValidationErrors["SignSuffix"] = "Укажите текст объявления о событии";
                this.TextValidationTextVisibility = true;
            }

            if (!settingsProvider.GroupId.HasValue)
            {
                this.ValidationErrors["GroupId"] = "Укажите группу";
            }

            RaisePropertyChanged(() => this.TextValidationTextVisibility);
        }
    }
}
