using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class OpenUserTemplateCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static OpenUserTemplateCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Utility.EditFile(UserTemplate.TemplateFile);
        }
    }
}