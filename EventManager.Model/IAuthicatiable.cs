using Windows.ApplicationModel.Activation;

namespace EventManager.Model
{
    public interface IAuthicatiable
    {
        bool IsLoginCompleted { get; }

        bool CompleteAuth(WebAuthenticationBrokerContinuationEventArgs args);
        void RequestLogin();
    }
}
