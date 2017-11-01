using System.Threading.Tasks;
using System.Windows;

namespace Infrastructure
{
    public interface INotify
    {
        Task<bool> Alert(string message);
        Task<MessageBoxResult> ConfirmYesNo(string question);
        Task<MessageBoxResult> ConfirmYesNoCancel(string question);
    }
}