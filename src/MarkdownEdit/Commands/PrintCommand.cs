using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class PrintCommand
    {
        public static readonly RoutedCommand Command = ApplicationCommands.Print;

        static PrintCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Preview.Print();
        }
    }
}