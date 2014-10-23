using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand WordWrapCommand = new RoutedUICommand();
        public static RoutedCommand InsertHeaderCommand = new RoutedUICommand();
        public static RoutedCommand FindNextCommand = new RoutedUICommand();
        public static RoutedCommand FindPreviousCommand = new RoutedUICommand();
        public static RoutedCommand IncreaseFontSizeCommand = new RoutedUICommand();
        public static RoutedCommand DecreaseFontSizeCommand = new RoutedUICommand();
        public static RoutedCommand RestoreFontSizeCommand = new RoutedUICommand();
        public static RoutedCommand OpenUserSettingsCommand = new RoutedCommand();
        public static RoutedCommand OpenUserTemplateCommand = new RoutedCommand();

        public UserSettings UserSettings { get; set; }
        private FileSystemWatcher _userSettingsWatcher;
        private string _titleName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview(Editor.Text);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(e);
            InitiailzeUserSettings();
        }

        private void InitiailzeUserSettings()
        {
            UserSettings = UserSettings.Load();
            _userSettingsWatcher = new FileSystemWatcher
            {
                Path = UserSettings.SettingsFolder,
                Filter = Path.GetFileName(UserSettings.SettingsFile),
                NotifyFilter = NotifyFilters.LastWrite
            };
            _userSettingsWatcher.Changed += (sender, args) => UserSettings.Update();
            _userSettingsWatcher.EnableRaisingEvents = true;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = !Editor.SaveIfModified();
        }

        protected override void OnClosed(EventArgs e)
        {
            _userSettingsWatcher.Dispose();
            base.OnClosed(e);
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case "FileName":
                case "DisplayName":
                case "IsModified":
                    TitleName = BuildTitle();
                    break;
            }
        }

        private string BuildTitle()
        {
            return string.Format("MARKDOWN EDIT - {0}{1}", Editor.IsModified ? "*" : "", Editor.DisplayName);
        }

        // Commands

        private void ExecuteNewFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.NewFile();
        }

        private void CanExecuteNewFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.OpenFile();
        }

        private void CanExecuteOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        public void ExecuteSaveFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.SaveFile();
        }

        private void CanExecuteSaveFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        public void ExecuteWordWrap(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.WordWrap = !Editor.WordWrap;
        }

        public void ExecuteHelp(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.ToggleHelp();
        }

        public void ExecuteClose(object sender, ExecutedRoutedEventArgs ea)
        {
            Close();
        }

        private void ExecuteEditorFind(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.FindDialog();
        }

        private void ExecuteEditorReplace(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.ReplaceDialog();
        }

        private void ExecuteBold(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.Bold();
        }

        private void ExecuteItalic(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.Italic();
        }

        private void ExecuteInsertHeader(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.InsertHeader(Convert.ToInt32(ea.Parameter));
        }

        private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.FindNext();
        }

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.FindPrevious();
        }

        private void ExecuteIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.IncreaseFontSize();
        }

        private void ExecuteRestoreFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.RestoreFontSize();
        }

        private void ExecuteDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.DecreaseFontSize();
        }

        private void ExecuteOpenUserSettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("Notepad.exe", UserSettings.SettingsFile);
        }

        private void ExecuteOpenUserTemplateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("Notepad.exe", UserTemplate.TemplateFile);
        }

        // Properites

        public string TitleName
        {
            get { return _titleName; }
            set
            {
                if (_titleName == value) return;
                _titleName = value;
                OnPropertyChanged();
            }
        }

        // INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}