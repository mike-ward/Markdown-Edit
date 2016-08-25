using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MarkdownEdit.Commands
{
    internal static class GoToMarkdownEditWebSiteCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static GoToMarkdownEditWebSiteCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e) => 
            Process.Start(new ProcessStartInfo("http://markdownedit.com"));
    }
}
