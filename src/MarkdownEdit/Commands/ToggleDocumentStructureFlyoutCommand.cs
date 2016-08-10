using System.Windows.Input;
using MahApps.Metro.Controls;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal class ToggleDocumentStructureFlyoutCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var flyouts = ((MainWindow)sender).Flyouts;
            ((Flyout)flyouts.Items[0]).IsOpen = false;
            var structureFlyout = (Flyout)flyouts.Items[1];
            structureFlyout.IsOpen = !structureFlyout.IsOpen;
        }
    }
}
