using System.Windows.Input;
using MarkdownEdit.Commands;

namespace MarkdownEdit.Controls
{
    public partial class InsertHyperlinkDialog
    {
        public static RoutedCommand AcceptLinkCommand = new RoutedUICommand();

        public InsertHyperlinkDialog(string selectedText)
        {
            InitializeComponent();
            Link.Focus();
            Link.SelectAll();
            LinkTitle.Text = selectedText;
            CommandBindings.Add(new CommandBinding(AcceptLinkCommand, ExecuteAcceptLinkCommand));
        }

        private void ExecuteAcceptLinkCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var linkText = Link.Text;
            if (!string.IsNullOrWhiteSpace(linkText))
            {
                if (!string.IsNullOrWhiteSpace(LinkTitle.Text))
                {
                    linkText = $"{linkText} \"{LinkTitle.Text}\"";
                }
                InsertHyperlinkCommand.Command.Execute(linkText, Owner);
            }
            Close();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }
    }
}