using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class OpenUserSettingsCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static OpenUserSettingsCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Utility.EditFile(UserSettings.SettingsFile);
        }
    }
}