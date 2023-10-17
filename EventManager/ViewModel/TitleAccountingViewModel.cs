using EventManager.Model;
using EventManager.WPFExtentions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace EventManager.ViewModel
{
    internal class TitleAccountingViewModel : ViewModelBase
    {
        private IAccountingProvider accountingProvider;
        private INavigationService navigationService;


        public TitleAccountingViewModel(IAccountingProvider accountingProvider, INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.accountingProvider = accountingProvider;
            this.accountingProvider.PropertyChanged += AccountingProvider_PropertyChanged;
        }

        private void AccountingProvider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.RestForMounth);
            this.RaisePropertyChanged(() => this.BallsFund);
        }

        public string RestForMounth { get { return "4400"; } }

        public string BallsFund
        {
            get { return $" {this.accountingProvider.BallFundAmount} р"; }
        }

        public AsyncCommand PayForMounth
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    await this.accountingProvider.AddRentPaymentAsync();
                    this.RaisePropertyChanged(() => this.BallsFund);
                    this.RaisePropertyChanged(() => this.RestForMounth);
                });
            }
        }

        public RelayCommand AccountingDetails
        {
            get { return new RelayCommand((o) => navigationService.NavigateTo("/View/EventVisitors.xaml"), (o) => true); }
        }
    }

}
