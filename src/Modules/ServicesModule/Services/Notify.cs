using System.Windows;
using Infrastructure;

namespace ServicesModule.Services
{
    public class Notify : INotify
    {
        public void Alert(string alert, Window owner = null)
        {
            Show(alert, MessageBoxButton.OK, MessageBoxImage.Error, owner);
        }

        public MessageBoxResult ConfirmYesNo(string question, Window owner = null)
        {
            return Show(question, MessageBoxButton.YesNo, MessageBoxImage.Question, owner);
        }

        public MessageBoxResult ConfirmYesNoCancel(string question, Window owner = null)
        {
            return Show(question, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, owner);
        }

        private MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage image, Window owner)
        {
            var text = message ?? "null";
            var window = owner ?? Application.Current.MainWindow;
            Application.Current.MainWindow.Activate();

            return Application.Current.Dispatcher.Invoke(() => window != null
                ? MessageBox.Show(window, text, Constants.ProgramName, button, image)
                : MessageBox.Show(text, Constants.ProgramName, button, image));
        }
    }
}
