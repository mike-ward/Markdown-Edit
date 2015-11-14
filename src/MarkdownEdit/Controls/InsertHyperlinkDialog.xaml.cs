using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class InsertHyperlinkDialog
    {
        public static RoutedCommand AcceptLinkCommand = new RoutedUICommand();

        public InsertHyperlinkDialog()
        {
            InitializeComponent();
            Link.Focus();
            CommandBindings.Add(new CommandBinding(AcceptLinkCommand, ExecuteAcceptLinkCommand));
        }

        private void ExecuteAcceptLinkCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Link.Text))
            {
                Utility.Beep();
                return;
            }

            MainWindow.InsertHyperlinkCommand.Execute(Link.Text, Owner);
            Close();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }
    }
}