using System.Windows;

namespace Infrastructure
{
    public interface INotify
    {
        void Alert(string message, Window owner = null);
        MessageBoxResult ConfirmYesNo(string question, Window owner = null);
        MessageBoxResult ConfirmYesNoCancel(string question, Window owner = null);
    }
}