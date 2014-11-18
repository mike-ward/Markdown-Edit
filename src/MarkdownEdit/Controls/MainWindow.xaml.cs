using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand ToggleWordWrapCommand = new RoutedCommand();
        public static RoutedCommand InsertHeaderCommand = new RoutedCommand();
        public static RoutedCommand FindNextCommand = new RoutedCommand();
        public static RoutedCommand FindPreviousCommand = new RoutedCommand();
        public static RoutedCommand RestoreFontSizeCommand = new RoutedCommand();
        public static RoutedCommand OpenUserSettingsCommand = new RoutedCommand();
        public static RoutedCommand OpenUserTemplateCommand = new RoutedCommand();
        public static RoutedCommand OpenUserDictionaryCommand = new RoutedCommand();
        public static RoutedCommand ToggleSpellCheckCommand = new RoutedCommand();
        public static RoutedCommand ToggleFullScreenCommand = new RoutedCommand();
        public static RoutedCommand WrapToColumnCommand = new RoutedCommand();
        public static RoutedCommand RecentFilesCommand = new RoutedCommand();
        public static RoutedCommand PasteSpecialCommand = new RoutedCommand();
        public static RoutedCommand ToggleCodeCommand = new RoutedCommand();
        public static RoutedCommand TogglePreviewCommand = new RoutedCommand();
        public static RoutedCommand LoadThemeCommand = new RoutedCommand();
        public static RoutedCommand SaveThemeCommand = new RoutedCommand();
        public static RoutedCommand ShowThemeDialogCommand = new RoutedCommand();
        public static RoutedCommand ExportHtmlCommand = new RoutedCommand();
        public static RoutedCommand ScrollToLineCommand = new RoutedCommand();
        public static RoutedCommand ShowGotoLineDialogCommand = new RoutedCommand();

        private string _titleName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closing += OnClosing;
            SizeChanged += (s, e) => CalculateEditorMargins();
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview(Editor.Text);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
            LoadCommandLineOrLastFile();
        }

        private void LoadCommandLineOrLastFile()
        {
            var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault()
                ?? (App.UserSettings.EditorOpenLastFile ? Settings.Default.LastOpenFile : null);
            Editor.OpenFile(fileToOpen);  
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Editor.CloseHelp();
            cancelEventArgs.Cancel = !Editor.SaveIfModified();
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

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.OpenFile(ea.Parameter as string);
        }

        public void ExecuteSaveFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.SaveFile();
        }

        public void ExecuteSaveFileAs(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.SaveFileAs();
        }

        public void ExecuteToggleWordWrap(object sender, ExecutedRoutedEventArgs ea)
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

        private void ExecuteCode(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.Code();
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
            Utility.EditFile(UserSettings.SettingsFile);
        }

        private void ExecuteOpenUserTemplateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Utility.EditFile(UserTemplate.TemplateFile);
        }

        private void ExecuteOpenUserDictionaryCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.OpenUserDictionary();
        }

        private void ExecuteToggleSpellCheck(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.SpellCheck = !Editor.SpellCheck;
        }

        private void ExecuteToggleFullScreen(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void ExecuteRecentFiles(object sender, ExecutedRoutedEventArgs e)
        {
            RecentFilesDialog.Display(this);
        }

        private void ExecutePasteSpecial(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.PasteSpecial();
        }

        private void ExecuteTogglePreview(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.EditPreviewHide = (Settings.Default.EditPreviewHide + 1) % 3;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
        }

        private void UpdateEditorPreviewVisibility(int state)
        {
            switch (state)
            {
                case 1:
                    UniformGrid.Columns = 1;
                    Preview.Visibility = Visibility.Collapsed;
                    Editor.Visibility = Visibility.Visible;
                    break;

                case 2:
                    UniformGrid.Columns = 1;
                    Preview.Visibility = Visibility.Visible;
                    Editor.Visibility = Visibility.Collapsed;
                    break;

                default:
                    UniformGrid.Columns = 2;
                    Preview.Visibility = Visibility.Visible;
                    Editor.Visibility = Visibility.Visible;
                    break;
            }

            EditorMargins = CalculateEditorMargins();
        }

        private void ExecuteLoadTheme(object sender, ExecutedRoutedEventArgs e)
        {
            App.UserSettings.Theme = e.Parameter as Theme;
        }

        private void ExecuteSaveTheme(object sender, ExecutedRoutedEventArgs e)
        {
            App.UserSettings.Save();
        }

        private void ExecuteShowThemeDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new ThemeDialog {Owner = this, CurrentTheme = App.UserSettings.Theme};
            dialog.ShowDialog();
        }

        private Thickness CalculateEditorMargins()
        {
            var margin = (UniformGrid.Columns == 1) ? Width / 4 : 0;
            return new Thickness(margin, 0, margin, 0);
        }

        private void ExecuteExportHtml(object sender, ExecutedRoutedEventArgs e)
        {
            Utility.ExportHtmlToClipboard(Editor.Text);
        }

        private void ExecuteShowGotoLineDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new GotoLineDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void ExecuteScrollToLine(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;
            Editor.ScrollToLine((int)e.Parameter);
        }

        // Properites

        public string TitleName
        {
            get { return _titleName; }
            set { Set(ref _titleName, value); }
        }

        private Thickness _editorMargins;

        public Thickness EditorMargins
        {
            get { return _editorMargins; }
            set { Set(ref _editorMargins, value); }
        }

        // INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}