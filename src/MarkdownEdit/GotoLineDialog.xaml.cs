using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class GotoLineDialog
    {
        public static RoutedCommand AcceptLineNumberCommand = new RoutedUICommand();

        public GotoLineDialog()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(AcceptLineNumberCommand, ExecuteAcceptLineNumber));
        }

        private void ExecuteAcceptLineNumber(object sender, ExecutedRoutedEventArgs e)
        {
            int number;
            if (int.TryParse(Line.Text, out number))
                MainWindow.ScrollToLineCommand.Execute(number, Owner);
            else
                Utility.Beep();
        }
    }
}
