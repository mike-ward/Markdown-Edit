using System.Diagnostics;
using System.Windows.Input;

namespace MarkdownEdit.Commands
{
    internal class GoToMarkdownEditWebSiteCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void Execute(object sender, ExecutedRoutedEventArgs e) => Process.Start(new ProcessStartInfo("http://markdownedit.com"));
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
    }
}
