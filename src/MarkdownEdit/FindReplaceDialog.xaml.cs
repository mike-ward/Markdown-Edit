using System;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace MarkdownEdit
{
    public partial class FindReplaceDialog
    {
        private static string _textToFind = string.Empty;
        private static bool _caseSensitive = true;
        private static bool _wholeWord = true;
        private static bool _useRegex;
        private static bool _useWildcards;
        private static bool _searchUp;
        private readonly TextEditor _editor;
        private static FindReplaceDialog _dialog;

        public FindReplaceDialog(TextEditor editor)
        {
            InitializeComponent();
            _editor = editor;
            txtFind.Text = txtFind2.Text = _textToFind;
            cbCaseSensitive.IsChecked = _caseSensitive;
            cbWholeWord.IsChecked = _wholeWord;
            cbRegex.IsChecked = _useRegex;
            cbWildcards.IsChecked = _useWildcards;
            cbSearchUp.IsChecked = _searchUp;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            _textToFind = txtFind2.Text;
            _caseSensitive = (cbCaseSensitive.IsChecked == true);
            _wholeWord = (cbWholeWord.IsChecked == true);
            _useRegex = (cbRegex.IsChecked == true);
            _useWildcards = (cbWildcards.IsChecked == true);
            _searchUp = (cbSearchUp.IsChecked == true);
            _dialog = null;
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
            if (MessageBox.Show("Are you sure you want to Replace All occurences of \"" +
                                txtFind2.Text + "\" with \"" + txtReplace.Text + "\"?",
                "Replace All", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
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
                match = regex.Match(_editor.Text, regex.Options.HasFlag(RegexOptions.RightToLeft) ? _editor.Text.Length : 0);
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

        public static void ShowForReplace(TextEditor editor)
        {
            if (_dialog == null)
            {
                _dialog = new FindReplaceDialog(editor) {tabMain = {SelectedIndex = 1}};
                _dialog.Show();
                _dialog.Activate();
            }
            else
            {
                _dialog.tabMain.SelectedIndex = 1;
                _dialog.Activate();
            }

            if (!editor.TextArea.Selection.IsMultiline)
            {
                _dialog.txtFind.Text = _dialog.txtFind2.Text = editor.TextArea.Selection.GetText();
                _dialog.txtFind.SelectAll();
                _dialog.txtFind2.SelectAll();
                _dialog.txtFind2.Focus();
            }
        }
    }
}