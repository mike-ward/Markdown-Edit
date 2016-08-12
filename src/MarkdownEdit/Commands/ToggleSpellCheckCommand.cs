using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Commands
{
    internal static class ToggleSpellCheckCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleSpellCheckCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.SpellCheckEnabled = !Settings.Default.SpellCheckEnabled;
        }
    }
}