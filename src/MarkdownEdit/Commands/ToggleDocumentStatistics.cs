using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleDocumentStatistics
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleDocumentStatistics()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;

            switch (mainWindow.Preview.DocumentStatisticMode)
            {
                default:
                // ReSharper disable once RedundantCaseLabel
                case Preview.StatisticMode.Character:
                    mainWindow.Preview.DocumentStatisticMode = Preview.StatisticMode.Word;
                    break;
                case Preview.StatisticMode.Word:
                    mainWindow.Preview.DocumentStatisticMode = Preview.StatisticMode.Page;
                    break;
                case Preview.StatisticMode.Page:
                    mainWindow.Preview.DocumentStatisticMode = Preview.StatisticMode.Character;
                    break;
            }

            mainWindow.Preview.UpdateDocumentStatisticDisplayText();
        }
    }
}