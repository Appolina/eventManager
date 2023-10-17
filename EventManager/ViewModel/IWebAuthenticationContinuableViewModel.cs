using Windows.ApplicationModel.Activation;

namespace EventManager.ViewModel
{
    internal interface IWebAuthenticationContinuableViewModel
    {
        void CompleteAuth(WebAuthenticationBrokerContinuationEventArgs args);
    }
}