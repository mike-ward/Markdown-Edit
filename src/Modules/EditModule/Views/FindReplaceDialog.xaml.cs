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
            _theDialog = _theDialog ?? new FindReplaceDialog(editor) { Owner = Application.Current.MainWindow };
            _theDialog._tabMain.SelectedIndex = 0;
            _theDialog.Show();
            _theDialog.Activate();
            _theDialog._textFind.Focus();
        }
    }
}
