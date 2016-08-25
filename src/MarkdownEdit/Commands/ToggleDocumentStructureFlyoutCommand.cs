using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleDocumentStructureFlyoutCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleDocumentStructureFlyoutCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var flyouts = ((MainWindow)sender).Flyouts;
            ((Flyout)flyouts.Items[0]).IsOpen = false;
            var structureFlyout = (Flyout)flyouts.Items[1];
            structureFlyout.IsOpen = !structureFlyout.IsOpen;
        }
    }
}
