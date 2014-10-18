using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace MarkdownEdit
{
    public partial class FindReplaceDialog
    {
        private readonly TextEditor _editor;
        private static FindReplaceDialog _dialog;
        private static readonly FindReplaceSettings _findReplaceSettings = new FindReplaceSettings();

        public FindReplaceDialog(TextEditor editor)
        {
            InitializeComponent();
            DataContext = _findReplaceSettings;
            _editor = editor;
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind.Text)) SystemSounds.Beep.Play();
        }

        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind2.Text)) SystemSounds.Beep.Play();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            var regex = GetRegEx(txtFind2.Text);
            var input = _editor.Text.Substring(_editor.SelectionStart, _editor.SelectionLength);
            var match = regex.Match(input);
            var replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                _editor.Document.Replace(_editor.SelectionStart, _editor.SelectionLength, txtReplace.Text);
                replaced = true;
            }

            if (!FindNext(txtFind2.Text) && !replaced) SystemSounds.Beep.Play();
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            var regex = GetRegEx(txtFind2.Text, true);
            var offset = 0;
            _editor.BeginChange();
            foreach (Match match in regex.Matches(_editor.Text))
            {
                _editor.Document.Replace(offset + match.Index, match.Length, txtReplace.Text);
                offset += txtReplace.Text.Length - match.Length;
            }
            _editor.EndChange();
        }

        private bool FindNext(string textToFind)
        {
            var regex = GetRegEx(textToFind);
            var start = regex.Options.HasFlag(RegexOptions.RightToLeft)
                ? _editor.SelectionStart
                : _editor.SelectionStart + _editor.SelectionLength;

            var match = regex.Match(_editor.Text, start);
            if (!match.Success) // start again from beginning or end
            {
                match = regex.Match(_editor.Text,
                    regex.Options.HasFlag(RegexOptions.RightToLeft)
                        ? _editor.Text.Length
                        : 0);
            }

            if (match.Success)
            {
                _editor.Select(match.Index, match.Length);
                var loc = _editor.Document.GetLocation(match.Index);
                _editor.ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        private Regex GetRegEx(string textToFind, bool leftToRight = false)
        {
            var options = RegexOptions.None;
            if (cbSearchUp.IsChecked == true && !leftToRight) options |= RegexOptions.RightToLeft;
            if (cbCaseSensitive.IsChecked == false) options |= RegexOptions.IgnoreCase;
            if (cbRegex.IsChecked == true) return new Regex(textToFind, options);
            var pattern = Regex.Escape(textToFind);
            if (cbWildcards.IsChecked == true) pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
            if (cbWholeWord.IsChecked == true) pattern = "\\b" + pattern + "\\b";
            return new Regex(pattern, options);
        }

        public static void ShowFindDialog(TextEditor editor)
        {
            ShowDialog(editor);
        }

        public static void ShowReplaceDialog(TextEditor editor)
        {
            ShowDialog(editor, 1);
        }

        private static void ShowDialog(TextEditor editor, int index = 0)
        {
            _dialog = _dialog ?? new FindReplaceDialog(editor);
            _dialog.tabMain.SelectedIndex = index;
            _dialog.Show();
            _dialog.Activate();

            if (!editor.TextArea.Selection.IsMultiline)
            {
                _dialog.txtFind.Text = _dialog.txtFind2.Text = editor.TextArea.Selection.GetText();
                _dialog.txtFind.SelectAll();
                _dialog.txtFind2.SelectAll();
            }

            _dialog.Dispatcher.InvokeAsync(() => editor.TextArea.Selection.IsMultiline
                ? _dialog.txtFind2.Focus()
                : _dialog.txtFind.Focus());
        }

        public static void CloseDialog()
        {
            if (_dialog != null) _dialog.Close();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            CloseDialog();
        }

        public static void SaveSettings()
        {
            _findReplaceSettings.Save();
        }
    }
}