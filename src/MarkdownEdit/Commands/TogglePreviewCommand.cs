using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Commands
{
    internal static class TogglePreviewCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static TogglePreviewCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            Settings.Default.EditPreviewHide = (Settings.Default.EditPreviewHide + 1) % 3;
            mainWindow.UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
        }
    }
}