using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand ToggleWordWrapCommand = new RoutedCommand();
        public static RoutedCommand InsertHeaderCommand = new RoutedCommand();
        public static RoutedCommand FindNextCommand = new RoutedCommand();
        public static RoutedCommand FindPreviousCommand = new RoutedCommand();
        public static RoutedCommand RestoreFontSizeCommand = new RoutedCommand();
        public static RoutedCommand OpenUserSettingsCommand = new RoutedCommand();
        public static RoutedCommand OpenKeybindingSettingsCommand = new RoutedCommand();
        public static RoutedCommand OpenUserTemplateCommand = new RoutedCommand();
        public static RoutedCommand OpenUserDictionaryCommand = new RoutedCommand();
        public static RoutedCommand OpenUserSnippetsCommand = new RoutedCommand();
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
        public static RoutedCommand ExportHtmlTemplateCommand = new RoutedCommand();
        public static RoutedCommand ScrollToLineCommand = new RoutedCommand();
        public static RoutedCommand ShowGotoLineDialogCommand = new RoutedCommand();
        public static RoutedCommand ToggleAutoSaveCommand = new RoutedCommand();
        public static RoutedCommand SelectPreviousHeaderCommand = new RoutedCommand();
        public static RoutedCommand SelectNextHeaderCommand = new RoutedCommand();
        public static RoutedCommand EditorFindCommand = new RoutedCommand();
        public static RoutedCommand EditorReplaceCommand = new RoutedCommand();
        public static RoutedCommand EditorReplaceAllCommand = new RoutedCommand();
        public static RoutedCommand OpenNewInstanceCommand = new RoutedCommand();
        public static RoutedCommand UpdatePreviewCommand = new RoutedCommand();
        public static RoutedCommand InsertFileCommand = new RoutedCommand();

        private string _titleName = string.Empty;
        private IMarkdownConverter _markdownConverter;
        private ISpellCheckProvider _spellCheckProvider;
        private FindReplaceDialog _findReplaceDialog;
        private ISnippetManager _snippetManager;

        public MainWindow()
        {
            // for designer
        }

        public MainWindow(
            IMarkdownConverter markdownConverter,
            ISpellCheckProvider spellCheckProvider,
            ISnippetManager snippetManager)
        {
            InitializeComponent();
            MarkdownConverter = markdownConverter;
            SpellCheckProvider = spellCheckProvider;
            SnippetManager = snippetManager;
            Loaded += OnLoaded;
            Closing += OnClosing;
            SizeChanged += (s, e) => CalculateEditorMargins();
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview(((Editor)s));
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Activate();
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
            Dispatcher.InvokeAsync(() =>
            {
                InputKeyBindingsSettings.Update();
                LoadCommandLineOrLastFile();
            });
        }

        private void LoadCommandLineOrLastFile()
        {
            var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault() ??
                             (App.UserSettings.EditorOpenLastFile ? Settings.Default.LastOpenFile : null);
            if (fileToOpen != "-n") Editor.LoadFile(fileToOpen);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Editor.CloseHelp();
            DisplaySettings.SaveIfModified();
            cancelEventArgs.Cancel = !Editor.SaveIfModified();
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case nameof(Editor.FileName):
                case nameof(Editor.DisplayName):
                case nameof(Editor.IsModified):
                    TitleName = BuildTitle();
                    break;
            }
        }

        private void SettingsClosingFinished(object sender, RoutedEventArgs e) => DisplaySettings.SaveIfModified();

        private string BuildTitle() => string.Format("MARKDOWN EDIT - {0}{1}", Editor.IsModified ? "* " : "", Editor.DisplayName);

        // Commands

        private void ExecuteNewFile(object sender, ExecutedRoutedEventArgs ea) => Editor.NewFile();

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea) => Editor.OpenFile(ea.Parameter as string);

        public void ExecuteSaveFile(object sender, ExecutedRoutedEventArgs ea) => Editor.SaveFile();

        public void ExecuteSaveFileAs(object sender, ExecutedRoutedEventArgs ea) => Editor.SaveFileAs();

        public void ExecuteToggleWordWrap(object sender, ExecutedRoutedEventArgs ea) => Settings.Default.WordWrapEnabled = !Settings.Default.WordWrapEnabled;

        public void ExecuteHelp(object sender, ExecutedRoutedEventArgs ea) => Editor.ToggleHelp();

        public void ExecuteClose(object sender, ExecutedRoutedEventArgs ea) => Close();

        private void ExecuteEditorFind(object sender, ExecutedRoutedEventArgs e) => Editor.FindDialog();

        private void ExecuteEditorReplace(object sender, ExecutedRoutedEventArgs e) => Editor.ReplaceDialog();

        private void ExecuteBold(object sender, ExecutedRoutedEventArgs ea) => Editor.Bold();

        private void ExecuteItalic(object sender, ExecutedRoutedEventArgs ea) => Editor.Italic();

        private void ExecuteCode(object sender, ExecutedRoutedEventArgs ea) => Editor.Code();

        private void ExecuteInsertHeader(object sender, ExecutedRoutedEventArgs ea) => Editor.InsertHeader(Convert.ToInt32(ea.Parameter));

        private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e) => Editor.FindNext();

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e) => Editor.FindPrevious();

        private void ExecuteIncreaseFontSize(object sender, ExecutedRoutedEventArgs e) => Editor.IncreaseFontSize();

        private void ExecuteRestoreFontSize(object sender, ExecutedRoutedEventArgs e) => Editor.RestoreFontSize();

        private void ExecuteDecreaseFontSize(object sender, ExecutedRoutedEventArgs e) => Editor.DecreaseFontSize();

        private void ExecuteOpenUserSettingsCommand(object sender, ExecutedRoutedEventArgs e) => Utility.EditFile(UserSettings.SettingsFile);

        private void ExecuteOpenKeybindingSettingsCommand(object sender, ExecutedRoutedEventArgs e) => Utility.EditFile(InputKeyBindingsSettings.KeyBindingFile);

        private void ExecuteOpenUserTemplateCommand(object sender, ExecutedRoutedEventArgs e) => Utility.EditFile(UserTemplate.TemplateFile);

        private void ExecuteOpenUserDictionaryCommand(object sender, ExecutedRoutedEventArgs e) => Editor.OpenUserDictionary();

        private void ExecuteOpenUserSnippetsCommand(object sender, ExecutedRoutedEventArgs e) => Utility.EditFile(Snippets.SnippetManager.SnippetFile());

        private void ExecuteToggleSpellCheck(object sender, ExecutedRoutedEventArgs e) => Settings.Default.SpellCheckEnabled = !Settings.Default.SpellCheckEnabled;

        private void ExecuteToggleFullScreen(object sender, ExecutedRoutedEventArgs e)
        {
            var control = Keyboard.FocusedElement;
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            SetFocus(control);
        }

        private void ExecuteRecentFiles(object sender, ExecutedRoutedEventArgs e) => RecentFilesDialog.Display(this);

        private void ExecutePasteSpecial(object sender, ExecutedRoutedEventArgs e) => Editor.PasteSpecial();

        private void ExecuteToggleAutoSave(object sender, ExecutedRoutedEventArgs e) => Settings.Default.AutoSave = !Settings.Default.AutoSave;

        private void ExecuteTogglePreview(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.EditPreviewHide = (Settings.Default.EditPreviewHide + 1) % 3;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
        }

        private void ExecuteSelectPreviousHeader(object sender, ExecutedRoutedEventArgs e) => Editor.SelectPreviousHeader();

        private void ExecuteSelectNextHeader(object sender, ExecutedRoutedEventArgs e) => Editor.SelectNextHeader();

        private void ExecuteEditorFindCommand(object sender, ExecutedRoutedEventArgs e) => Editor.Find(e.Parameter as Regex);

        private void ExecuteEditorReplaceCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var tuple = (Tuple<Regex, string>)e.Parameter;
            Editor.Replace(tuple.Item1, tuple.Item2);
        }

        private void ExecuteEditorReplaceAllCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var tuple = (Tuple<Regex, string>)e.Parameter;
            Editor.ReplaceAll(tuple.Item1, tuple.Item2);
        }

        private void ExecuteOpenNewInstance(object sender, ExecutedRoutedEventArgs e)
        {
            var process = new Process { StartInfo = { FileName = Utility.ExecutingAssembly(), Arguments = "-n" } };
            process.Start();
        }

        private void SetFocus(IInputElement control)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                control.Focus();
                Keyboard.Focus(control);
            }));
        }

        private void UpdateEditorPreviewVisibility(int state)
        {
            UniformGrid.Columns = state == 0 ? 2 : 1;
            Editor.Visibility = state == 2 ? Visibility.Collapsed : Visibility.Visible;
            PreviewAirspaceDecorator.Visibility = state == 1 ? Visibility.Collapsed : Visibility.Visible;
            SetFocus(state == 2 ? Preview.Browser as IInputElement : Editor.EditBox);
            EditorMargins = CalculateEditorMargins();
        }

        private void ExecuteLoadTheme(object sender, ExecutedRoutedEventArgs e) => App.UserSettings.Theme = e.Parameter as Theme;

        private void ExecuteSaveTheme(object sender, ExecutedRoutedEventArgs e) => App.UserSettings.Save();

        private void ExecuteShowThemeDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new ThemeDialog { Owner = this, CurrentTheme = App.UserSettings.Theme };
            dialog.ShowDialog();
        }

        private Thickness CalculateEditorMargins()
        {
            var margin = (UniformGrid.Columns == 1) ? Width / 4 : 0;
            return new Thickness(margin, 0, margin, 0);
        }

        private void ExecuteExportHtml(object sender, ExecutedRoutedEventArgs e) => Utility.ExportHtmlToClipboard(Editor.Text, MarkdownConverter);

        private void ExecuteExportHtmlTemplate(object sender, ExecutedRoutedEventArgs e) => Utility.ExportHtmlTemplateToClipboard(Editor.Text, MarkdownConverter);

        private void ExecuteShowGotoLineDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new GotoLineDialog { Owner = this };
            dialog.ShowDialog();
        }

        private void ExecuteScrollToLine(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null) Editor.ScrollToLine((int)e.Parameter);
        }

        private void ExecuteUpdatePreview(object sender, ExecutedRoutedEventArgs e) => Preview.UpdatePreview(Editor);

        private void ExecuteInsertFile(object sender, ExecutedRoutedEventArgs e) => Editor.InsertFile(null);

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

        public IMarkdownConverter MarkdownConverter
        {
            get { return _markdownConverter; }
            set { Set(ref _markdownConverter, value); }
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get { return _spellCheckProvider; }
            set { Set(ref _spellCheckProvider, value); }
        }

        public FindReplaceDialog FindReplaceDialog
        {
            get { return _findReplaceDialog; }
            set { Set(ref _findReplaceDialog, value); }
        }

        public ISnippetManager SnippetManager
        {
            get { return _snippetManager; }
            set { Set(ref _snippetManager, value); }
        }

        private void ToggleSettings(object sender, RoutedEventArgs e)
        {
            var settingsFlyout = (Flyout)Flyouts.Items[0];
            settingsFlyout.IsOpen = !settingsFlyout.IsOpen;
            if (settingsFlyout.IsOpen) DisplaySettings.SaveState();
        }

        // INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) == false)
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}