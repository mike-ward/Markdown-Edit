using System.Media;
using System.Windows;

namespace MarkdownEdit.Models
{
    internal static class Notify
    {
        public static void Alert(string alert, Window owner = null)
            => Show(alert, MessageBoxButton.OK, MessageBoxImage.Error, owner);

        public static MessageBoxResult ConfirmYesNo(string question, Window owner = null)
            => Show(question, MessageBoxButton.YesNo, MessageBoxImage.Question, owner);

        public static MessageBoxResult ConfirmYesNoCancel(string question, Window owner = null)
            => Show(question, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, owner);

        private static MessageBoxResult Show(string message, MessageBoxButton button, MessageBoxImage image,
            Window owner)
        {
            var text = message ?? "null";
            var window = owner ?? Application.Current.MainWindow;

            return Application.Current.Dispatcher.Invoke(() => window != null
                ? MessageBox.Show(window, text, App.Title, button, image)
                : MessageBox.Show(text, App.Title, button, image));
        }

        public static void Beep() => SystemSounds.Beep.Play();
    }
}