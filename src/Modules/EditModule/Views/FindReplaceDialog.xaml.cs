using System.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Views
{
    public partial class FindReplaceDialog
    {
        private readonly TextEditor _textEditor;
        private static FindReplaceDialog _dialog;

        private FindReplaceDialogViewModel ViewModel => (FindReplaceDialogViewModel)DataContext;

        public FindReplaceDialog(TextEditor textEditor)
        {
            _textEditor = textEditor;
            InitializeComponent();

            Loaded += ViewModel.OnLoad;
            Closed += ViewModel.OnClose;
            Closed += (sd, ea) => _dialog = null;
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

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void TextFindOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _findNextButton.Focus();
                _findNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        public static void ShowFindReplace(TextEditor editor, bool replace = false)
        {
            _dialog = _dialog ?? new FindReplaceDialog(editor) { Owner = Application.Current.MainWindow };
            _dialog._tabMain.SelectedIndex = replace ? 1 : 0;
            _dialog.Show();
            _dialog.Activate();
            if (replace) _dialog._textFind.Focus();
            else _dialog._textFind.Focus();
        }
    }
}
