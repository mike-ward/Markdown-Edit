using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using MarkdownEdit.Commands;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;
using Version = MarkdownEdit.Models.Version;

namespace MarkdownEdit.Controls
{
    public partial class MainWindow
    {
        public const int EditorMarginMin = 4;
        public const int EditorMarginMax = 16;
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            // for designer
        }

        public MainWindow(
            ISpellCheckProvider spellCheckProvider,
            ISnippetManager snippetManager)
        {
            InitializeComponent();

            _vm = (MainWindowViewModel)DataContext;
            _vm.SpellCheckProvider = spellCheckProvider;
            _vm.SnippetManager = snippetManager;

            Closing += OnClosing;
            Activated += OnFirstActivation;
            IsVisibleChanged += OnIsVisibleChanged;
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview((Editor)s);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(s, e);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
            Activate();
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
                    _vm.TitleName = $"{App.Title} - {(Editor.IsModified ? "* " : "")}{Editor.DisplayName}";
                    break;
            }
        }

        private void OnFirstActivation(object sender, EventArgs eventArgs)
        {
            Activated -= OnFirstActivation;
            Dispatcher.InvokeAsync(async () =>
            {
                var updateMargins = Utility.Debounce(() =>
                    Dispatcher.Invoke(() => _vm.EditorMargins = CalculateEditorMargins()), 50);

                App.UserSettings.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == nameof(App.UserSettings.SinglePaneMargin)) updateMargins();
                    if (args.PropertyName == nameof(App.UserSettings.GitHubMarkdown)) Preview.UpdatePreview(Editor);
                };
                SizeChanged += (s, e) => updateMargins();
                updateMargins();
                LoadCommandLineOrLastFile();
                Application.Current.Activated += OnActivated;
                _vm.NewVersion = !await Version.IsCurrentVersion();
            });
        }

        private void OnActivated(object sender, EventArgs args)
        {
            if (Preview.Visibility == Visibility.Visible && Editor.Visibility != Visibility.Visible)
            {
                SetFocus(Preview.Browser);
            }
        }

        private void SettingsClosingFinished(object sender, RoutedEventArgs e)
        {
            DisplaySettings.SaveIfModified();
        }

        // Stuff that should probably go elsewhere

        private void LoadCommandLineOrLastFile()
        {
            var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault()
                ?? (App.UserSettings.EditorOpenLastFile ? Settings.Default.LastOpenFile : null);

            if (string.IsNullOrWhiteSpace(fileToOpen)
                || fileToOpen == "-n"
                || !Editor.LoadFile(fileToOpen))
            {
                NewFileCommand.Command.Execute(null, this);
            }
        }

        public void SetFocus(IInputElement control)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
            {
                control.Focus();
                Keyboard.Focus(control);
            }));
        }

        public void UpdateEditorPreviewVisibility(int state)
        {
            Editor.Visibility = state == 2 ? Visibility.Collapsed : Visibility.Visible;
            PreviewAirspaceDecorator.Visibility = state == 1 ? Visibility.Collapsed : Visibility.Visible;
            Preview.Visibility = PreviewAirspaceDecorator.Visibility;
            UniformGrid.Columns = state == 0 ? 2 : 1;
            SetFocus(state == 2 ? Preview.Browser as IInputElement : Editor.EditBox);
            _vm.EditorMargins = CalculateEditorMargins();
        }

        public Thickness CalculateEditorMargins()
        {
            var singlePaneMargin = Math.Min(Math.Max(EditorMarginMin, App.UserSettings.SinglePaneMargin), EditorMarginMax);
            var margin = UniformGrid.Columns == 1 ? ActualWidth / singlePaneMargin : 0;
            return new Thickness(margin, 0, margin, 0);
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
    }
}