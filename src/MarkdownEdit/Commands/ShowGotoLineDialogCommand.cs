using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ShowGotoLineDialogCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ShowGotoLineDialogCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            new GotoLineDialog { Owner = (MainWindow)sender }.ShowDialog();
        }
    }
}