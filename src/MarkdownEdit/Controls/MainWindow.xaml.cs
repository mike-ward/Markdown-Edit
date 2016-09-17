using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using MarkdownEdit.Models;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public partial class MainWindow
    {
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
            _vm.MainWindow = this;
            _vm.SpellCheckProvider = spellCheckProvider;
            _vm.SnippetManager = snippetManager;

            Closing += _vm.OnClosing;
            Activated += _vm.OnFirstActivation;
            IsVisibleChanged += _vm.OnIsVisibleChanged;
            Editor.PropertyChanged += _vm.EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview((Editor)s);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(s, e);
        }

        private void SettingsClosingFinished(object sender, RoutedEventArgs e)
        {
            DisplaySettings.SaveIfModified();
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
            _vm.UpdateEditorPreviewVisibility(state);
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