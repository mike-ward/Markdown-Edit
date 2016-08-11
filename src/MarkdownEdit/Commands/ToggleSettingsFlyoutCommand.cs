using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleSettingsFlyoutCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleSettingsFlyoutCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            var flyouts = mainWindow.Flyouts;
            ((Flyout)flyouts.Items[1]).IsOpen = false;
            var settingsFlyout = (Flyout)flyouts.Items[0];
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
            if (settingsFlyout.IsOpen) mainWindow.DisplaySettings.SaveState();
        }
    }
}
