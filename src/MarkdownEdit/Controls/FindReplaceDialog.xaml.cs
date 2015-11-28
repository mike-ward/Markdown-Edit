using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class FindReplaceDialog : INotifyPropertyChanged
    {
        private string _findText;
        private FindReplaceSettings _findReplaceSettings;

        public FindReplaceDialog(FindReplaceSettings findReplaceSettings)
        {
            InitializeComponent();
            DataContext = this;
            _findReplaceSettings = findReplaceSettings;
            Closed += (s, e) => findReplaceSettings.Save();
            Closing += (s, e) => { e.Cancel = true; Hide(); };
        }

        public string FindText
        {
            get { return _findText; }
            set { Set(ref _findText, value);}
        }

        public FindReplaceSettings FindReplaceSettings
        {
            get { return _findReplaceSettings; }
            set { Set(ref _findReplaceSettings, value); }
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _findText = FindText;
                var find = GetRegEx(_findText, false);
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
                _findText = FindText;
                var find = GetRegEx(_findText, false);
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
                _findText = FindText;
                var find = GetRegEx(_findText, false);
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
                _findText = FindText;
                var find = GetRegEx(_findText, false);
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
            var options = RegexOptions.Multiline;
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

        public void FindPrevious()
        {
            if (_findText == null) return;
            var find = GetRegEx(_findText, true);
            MainWindow.EditorFindCommand.Execute(find, Application.Current.MainWindow);
        }

        public void FindNext()
        {
            if (_findText == null) return;
            var find = GetRegEx(_findText, false);
            MainWindow.EditorFindCommand.Execute(find, Application.Current.MainWindow);
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }

        private void TxtFindOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) FindNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}