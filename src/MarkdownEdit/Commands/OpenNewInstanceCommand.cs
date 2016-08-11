using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class OpenNewInstanceCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static OpenNewInstanceCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            new Process { StartInfo = { FileName = Utility.ExecutingAssembly(), Arguments = "-n" } }.Start();
        }
    }
}