using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class UpdatePreviewCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static UpdatePreviewCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Preview.UpdatePreview(mainWindow.Editor);
        }
    }
}