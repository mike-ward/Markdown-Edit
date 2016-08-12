using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class ScrollToLineCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ScrollToLineCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            if (e.Parameter != null) EditorUtilities.ScrollToLine(mainWindow.Editor.EditBox, (int)e.Parameter);

        }
    }
}