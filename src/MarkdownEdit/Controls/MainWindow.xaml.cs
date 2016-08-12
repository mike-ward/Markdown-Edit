using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;
using Clipboard = MarkdownEdit.Models.Clipboard;
using Version = MarkdownEdit.Models.Version;

namespace MarkdownEdit.Controls
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public const int EditorMarginMin = 4;
        public const int EditorMarginMax = 16;
        public static readonly RoutedCommand InsertHeaderCommand = new RoutedCommand();
        public static readonly RoutedCommand RestoreFontSizeCommand = new RoutedCommand();
        public static readonly RoutedCommand OpenUserSettingsCommand = new RoutedCommand();
        public static readonly RoutedCommand OpenUserTemplateCommand = new RoutedCommand();
        public static readonly RoutedCommand OpenUserDictionaryCommand = new RoutedCommand();
        public static readonly RoutedCommand OpenUserSnippetsCommand = new RoutedCommand();
        public static readonly RoutedCommand ToggleSpellCheckCommand = new RoutedCommand();
        public static readonly RoutedCommand ToggleDocumentStatistics = new RoutedCommand();
        public static readonly RoutedCommand ToggleFullScreenCommand = new RoutedCommand();
        public static readonly RoutedCommand WrapToColumnCommand = new RoutedCommand();
        public static readonly RoutedCommand RecentFilesCommand = new RoutedCommand();
        public static readonly RoutedCommand ToggleCodeCommand = new RoutedCommand();
        public static readonly RoutedCommand TogglePreviewCommand = new RoutedCommand();
        public static readonly RoutedCommand ExportHtmlCommand = new RoutedCommand();
        public static readonly RoutedCommand ExportHtmlTemplateCommand = new RoutedCommand();
        public static readonly RoutedCommand SaveAsHtmlCommand = new RoutedCommand();
        public static readonly RoutedCommand SaveAsHtmlTemplateCommand = new RoutedCommand();

        private IMarkdownConverter _commonMarkConverter;

        private Thickness _editorMargins;
        private FindReplaceDialog _findReplaceDialog;
        private IMarkdownConverter _githubMarkdownConverter;

        private bool _newVersion;
        private ISnippetManager _snippetManager;
        private ISpellCheckProvider _spellCheckProvider;

        private string _titleName = string.Empty;

        public MainWindow()
        {
            // for designer
        }

        public MainWindow(
            ISpellCheckProvider spellCheckProvider,
            ISnippetManager snippetManager)
        {
            DataContext = this;
            InitializeComponent();
            SpellCheckProvider = spellCheckProvider;
            SnippetManager = snippetManager;
            Closing += OnClosing;
            Activated += OnFirstActivation;
            IsVisibleChanged += OnIsVisibleChanged;
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview((Editor)s);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(s, e);
        }

        // Properites

        public string TitleName { get { return _titleName; } set { Set(ref _titleName, value); } }

        public Thickness EditorMargins { get { return _editorMargins; } set { Set(ref _editorMargins, value); } }

        public IMarkdownConverter CommonMarkConverter
        {
            get { return _commonMarkConverter; }
            set { Set(ref _commonMarkConverter, value); }
        }

        public IMarkdownConverter GitHubMarkdownConverter
        {
            get { return _githubMarkdownConverter; }
            set { Set(ref _githubMarkdownConverter, value); }
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

        public bool NewVersion { get { return _newVersion; } set { Set(ref _newVersion, value); } }

        private void OnIsVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
            Activate();
        }

        private void OnFirstActivation(object sender, EventArgs eventArgs)
        {
            Activated -= OnFirstActivation;
            Dispatcher.InvokeAsync(async () =>
            {
                var updateMargins =
                    Utility.Debounce(() => Dispatcher.Invoke(() => EditorMargins = CalculateEditorMargins()), 50);
                App.UserSettings.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == nameof(App.UserSettings.SinglePaneMargin)) updateMargins();
                    if (args.PropertyName == nameof(App.UserSettings.GitHubMarkdown)) Preview.UpdatePreview(Editor);
                };
                SizeChanged += (s, e) => updateMargins();
                updateMargins();
                LoadCommandLineOrLastFile();
                Application.Current.Activated += OnActivated;
                NewVersion = !await Version.IsCurrentVersion();
                TitleBarTooltip();
            });
        }

        private void LoadCommandLineOrLastFile()
        {
            var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault()
                             ?? (App.UserSettings.EditorOpenLastFile ? Settings.Default.LastOpenFile : null);

            if (string.IsNullOrWhiteSpace(fileToOpen)
                || fileToOpen == "-n"
                || !Editor.LoadFile(fileToOpen))
            {
                Editor.NewFile();
            }
        }

        private void TitleBarTooltip()
        {
            var titleBar = GetTemplateChild("PART_TitleBar") as UIElement;
            var textBlock = titleBar?.GetDescendantByType<TextBlock>();
            var fileNameBinding = new Binding { Path = new PropertyPath("FileName"), Source = Editor };
            textBlock?.SetBinding(ToolTipProperty, fileNameBinding);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Editor.CloseHelp();
            DisplaySettings.SaveIfModified();
            cancelEventArgs.Cancel = !Editor.SaveIfModified();
            if (cancelEventArgs.Cancel || App.UserSettings.YesIDonated) return;
            var donate = new Donate { Owner = Application.Current.MainWindow };
            donate.ShowDialog();
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

        private void SettingsClosingFinished(object sender, RoutedEventArgs e)
            => DisplaySettings.SaveIfModified();

        private string BuildTitle()
            => $"{App.Title} - {(Editor.IsModified ? "* " : "")}{Editor.DisplayName}";

        // Commands

        private void ExecuteNewFile(object sender, ExecutedRoutedEventArgs ea)
            => Editor.NewFile();

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
            => Editor.OpenFile(ea.Parameter as string);

        public void ExecuteSaveFile(object sender, ExecutedRoutedEventArgs ea)
            => Editor.SaveFile();

        public void ExecuteSaveFileAs(object sender, ExecutedRoutedEventArgs ea)
            => Editor.SaveFileAs();

        public void ExecuteHelp(object sender, ExecutedRoutedEventArgs ea)
            => Editor.ToggleHelp();

        public void ExecuteClose(object sender, ExecutedRoutedEventArgs ea)
            => Close();

        private void ExecuteBold(object sender, ExecutedRoutedEventArgs ea)
            => Editor.Bold();

        private void ExecuteItalic(object sender, ExecutedRoutedEventArgs ea)
            => Editor.Italic();

        private void ExecuteCode(object sender, ExecutedRoutedEventArgs ea)
            => Editor.Code();

        private void ExecuteInsertHeader(object sender, ExecutedRoutedEventArgs ea)
            => Editor.InsertHeader(Convert.ToInt32(ea.Parameter));

        private void ExecuteIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
            => Editor.IncreaseFontSize();

        private void ExecuteRestoreFontSize(object sender, ExecutedRoutedEventArgs e)
            => Editor.RestoreFontSize();

        private void ExecuteDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
            => Editor.DecreaseFontSize();

        private void ExecuteOpenUserSettingsCommand(object sender, ExecutedRoutedEventArgs e)
            => Utility.EditFile(UserSettings.SettingsFile);

        private void ExecuteOpenUserTemplateCommand(object sender, ExecutedRoutedEventArgs e)
            => Utility.EditFile(UserTemplate.TemplateFile);

        private void ExecuteOpenUserDictionaryCommand(object sender, ExecutedRoutedEventArgs e)
            => Editor.OpenUserDictionary();

        private void ExecuteOpenUserSnippetsCommand(object sender, ExecutedRoutedEventArgs e)
            => Utility.EditFile(Snippets.SnippetManager.SnippetFile());

        private void ExecuteToggleSpellCheck(object sender, ExecutedRoutedEventArgs e)
            => Settings.Default.SpellCheckEnabled = !Settings.Default.SpellCheckEnabled;

        private void ExecuteToggleFullScreen(object sender, ExecutedRoutedEventArgs e)
        {
            var control = Keyboard.FocusedElement;
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            SetFocus(control);
        }

        private void ExecuteRecentFiles(object sender, ExecutedRoutedEventArgs e)
            => RecentFilesDialog.Display(this);

        private void ExecuteTogglePreview(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.EditPreviewHide = (Settings.Default.EditPreviewHide + 1) % 3;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
        }

        private void SetFocus(IInputElement control)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                control.Focus();
                Keyboard.Focus(control);
            }));
        }

        private void OnActivated(object sender, EventArgs args)
        {
            if (Preview.Visibility == Visibility.Visible && Editor.Visibility != Visibility.Visible)
            {
                SetFocus(Preview.Browser);
            }
        }

        private void UpdateEditorPreviewVisibility(int state)
        {
            Editor.Visibility = state == 2 ? Visibility.Collapsed : Visibility.Visible;
            PreviewAirspaceDecorator.Visibility = state == 1 ? Visibility.Collapsed : Visibility.Visible;
            Preview.Visibility = PreviewAirspaceDecorator.Visibility;
            UniformGrid.Columns = state == 0 ? 2 : 1;
            SetFocus(state == 2 ? Preview.Browser as IInputElement : Editor.EditBox);
            EditorMargins = CalculateEditorMargins();
        }

        private Thickness CalculateEditorMargins()
        {
            var singlePaneMargin = Math.Min(Math.Max(EditorMarginMin, App.UserSettings.SinglePaneMargin),
                EditorMarginMax);
            var margin = UniformGrid.Columns == 1 ? ActualWidth / singlePaneMargin : 0;
            return new Thickness(margin, 0, margin, 0);
        }

        private void ExecuteExportHtml(object sender, ExecutedRoutedEventArgs e)
            => Clipboard.ExportHtmlToClipboard(Editor.Text);

        private void ExecuteExportHtmlTemplate(object sender, ExecutedRoutedEventArgs e)
            => Clipboard.ExportHtmlToClipboard(Editor.Text, true);

        private void ExecuteSaveAsHtml(object sender, ExecutedRoutedEventArgs e)
            => EditorLoadSave.SaveFileAs(Editor, "html");

        private void ExecuteSaveAsHtmlTemplate(object sender, ExecutedRoutedEventArgs e)
            => EditorLoadSave.SaveFileAs(Editor, "html-with-template");

        private void ExecutePrintHtml(object sender, ExecutedRoutedEventArgs e)
            => Preview.Print();


        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Old School

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.RemoveHook(WndProc);
            base.OnClosing(e);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x4A /* WM_COPYDATA */)
            {
                handled = true;
                var filename = OldSchool.CopyDataStructToString(lParam);
                return Editor.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase)
                    ? new IntPtr(1)
                    : IntPtr.Zero;
            }
            return IntPtr.Zero;
        }

        private void ExecuteToggleDocumentStatistics(object sender, ExecutedRoutedEventArgs e)
        {
            switch (Preview.DocumentStatisticMode)
            {
                default:
                // ReSharper disable once RedundantCaseLabel
                case Preview.StatisticMode.Character:
                    Preview.DocumentStatisticMode = Preview.StatisticMode.Word;
                    break;
                case Preview.StatisticMode.Word:
                    Preview.DocumentStatisticMode = Preview.StatisticMode.Page;
                    break;
                case Preview.StatisticMode.Page:
                    Preview.DocumentStatisticMode = Preview.StatisticMode.Character;
                    break;
            }

            Preview.UpdateDocumentStatisticDisplayText();
        }
    }
}