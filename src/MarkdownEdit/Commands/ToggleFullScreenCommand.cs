using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleFullScreenCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleFullScreenCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            var control = Keyboard.FocusedElement;
            mainWindow.WindowState = mainWindow.WindowState == WindowState.Maximized 
                ? WindowState.Normal 
                : WindowState.Maximized;
            mainWindow.SetFocus(control);
        }
    }
}