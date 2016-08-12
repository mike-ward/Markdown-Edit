using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class EditorFindCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static EditorFindCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Editor.Find(e.Parameter as Regex);
        }
    }
}