using System.Media;
using System.Windows;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Views
{
    public partial class FindReplaceDialog
    {
        private readonly TextEditor _textEditor;
        private static FindReplaceDialog _theDialog;

        private FindReplaceDialogViewModel ViewModel => (FindReplaceDialogViewModel)DataContext;

        public FindReplaceDialog(TextEditor textEditor)
        {
            _textEditor = textEditor;
            InitializeComponent();

            Loaded += ViewModel.OnLoad;
            Closed += ViewModel.OnClose;
            Closed += (sd, ea) => _theDialog = null;
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.FindNext(_textEditor)) SystemSounds.Beep.Play();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Replace(_textEditor);
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            ViewModel.ReplaceAll(_textEditor);
        }

        public static void ShowFindReplace(TextEditor editor)
        {
            if (_theDialog == null)
            {
                _theDialog = new FindReplaceDialog(editor) { _tabMain = { SelectedIndex = 1 } };
                _theDialog.Show();
                _theDialog.Activate();
            }
            else
            {
                _theDialog._tabMain.SelectedIndex = 1;
                _theDialog.Activate();
            }

            if (!editor.TextArea.Selection.IsMultiline)
            {
                _theDialog.ViewModel.FindText = editor.TextArea.Selection.GetText();
                _theDialog._textFind.SelectAll();
                _theDialog._textFind2.SelectAll();
                _theDialog._textFind2.Focus();
            }
        }
    }
}
