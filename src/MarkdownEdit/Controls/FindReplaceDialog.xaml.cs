using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class FindReplaceDialog
    {
        public FindReplaceDialog(FindReplaceSettings findReplaceSettings)
        {
            InitializeComponent();
            DataContext = findReplaceSettings;
            Closed += (s, e) => findReplaceSettings.Save();
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var find = GetRegEx(txtFind.Text, false);
                MainWindow.EditorFindCommand.Execute(find, Application.Current.MainWindow);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var find = GetRegEx(txtFind2.Text, false);
                MainWindow.EditorFindCommand.Execute(find, Application.Current.MainWindow);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var find = GetRegEx(txtFind2.Text, false);
                var replace = txtReplace.Text;
                var tuple = new Tuple<Regex, string>(find, replace);
                MainWindow.EditorReplaceCommand.Execute(tuple, Application.Current.MainWindow);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var find = GetRegEx(txtFind2.Text, false);
                var replace = txtReplace.Text;
                var tuple = new Tuple<Regex, string>(find, replace);
                MainWindow.EditorReplaceAllCommand.Execute(tuple, Application.Current.MainWindow);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private Regex GetRegEx(string textToFind, bool previous)
        {
            var options = RegexOptions.None;
            if (previous) options |= RegexOptions.RightToLeft;
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

            if (index == 1) txtFind2.Focus();
            else txtFind.Focus();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}