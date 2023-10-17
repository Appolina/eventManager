using System.Threading.Tasks;
using System.Windows.Input;

namespace EventManager.WPFExtentions
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
