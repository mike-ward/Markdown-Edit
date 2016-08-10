using System.Windows.Input;
using MahApps.Metro.Controls;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    class ToggleSettingsFlyoutCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

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
