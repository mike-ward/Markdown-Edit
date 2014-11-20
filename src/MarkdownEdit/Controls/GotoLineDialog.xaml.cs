using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class GotoLineDialog
    {
        public static RoutedCommand AcceptLineNumberCommand = new RoutedUICommand();

        public GotoLineDialog()
        {
            InitializeComponent();
            Line.Focus();
            CommandBindings.Add(new CommandBinding(AcceptLineNumberCommand, ExecuteAcceptLineNumber));
        }

        private void ExecuteAcceptLineNumber(object sender, ExecutedRoutedEventArgs e)
        {
            int number;
            if (int.TryParse(Line.Text, out number))
            {
                MainWindow.ScrollToLineCommand.Execute(number, Owner);
                Close();
            }
            else
            {
                Utility.Beep();
            }
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }
    }
}