using EventManager.WPFExtentions;
using GalaSoft.MvvmLight;

namespace EventManager.ViewModel
{
    public abstract class ViewModelBaseWithValidation : ViewModelBase
    {

        protected ViewModelBaseWithValidation()
        {
            this.ValidationErrors = new ValidationErrors();
        }

        public ValidationErrors ValidationErrors { get; set; }


        public bool IsValid { get; private set; }

        public void Validate()
        {
            //this.ValidationErrors.Clear();
            this.ValidateSelf();
            this.IsValid = this.ValidationErrors.IsValid;
            this.RaisePropertyChanged(() => this.IsValid);
            this.RaisePropertyChanged(() => ValidationErrors);
        }

        protected abstract void ValidateSelf();
    }
}
