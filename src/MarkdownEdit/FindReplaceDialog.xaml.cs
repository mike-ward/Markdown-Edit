using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace MarkdownEdit
{
    public partial class FindReplaceDialog
    {
        public bool AllowClose { get; set; }
        private readonly TextEditor _editor;
        private readonly FindReplaceSettings _findReplaceSettings = new FindReplaceSettings();

        public FindReplaceDialog(TextEditor editor)
        {
            InitializeComponent();
            _editor = editor;
            DataContext = _findReplaceSettings;
            Closed += (s, e) => _findReplaceSettings.Save();

            Closing += (s, e) =>
            {
                if (AllowClose) return;
                Hide();
                e.Cancel = true;
            };
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

        public void ShowFindDialog()
        {
            ShowDialog();
        }

        public void ShowReplaceDialog()
        {
            ShowDialog(1);
        }

        private void ShowDialog(int index = 0)
        {
            tabMain.SelectedIndex = index;
            Owner = Application.Current.MainWindow;
            Show();

            if (!_editor.TextArea.Selection.IsMultiline)
            {
                txtFind.Text = txtFind2.Text = _editor.TextArea.Selection.GetText();
                txtFind.SelectAll();
                txtFind2.SelectAll();
            }

            if (index == 1) txtFind2.Focus();
            else txtFind.Focus();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}