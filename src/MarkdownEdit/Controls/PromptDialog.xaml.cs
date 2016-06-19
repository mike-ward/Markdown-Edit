using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MarkdownEdit.Controls
{
    public partial class PromptDialog
    {
        public PromptDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private string Question { get; set; }
        private string ResponseText => Response.Text;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Title = App.Title;
            QuestionText.Text = Question;
            Response.Focus();
        }

        public static string Prompt(string question)
        {
            var prompt = new PromptDialog
            {
                Question = question
            };
            prompt.ShowDialog();
            return prompt.DialogResult == true ? prompt.ResponseText : null;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void TxtFindOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton.Focus();
                OkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}