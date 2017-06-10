using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using MarkdownEdit.Commands;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;
using Version = MarkdownEdit.Models.Version;

namespace MarkdownEdit.Controls
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public const int EditorMarginMin = 4;
        public const int EditorMarginMax = 16;

        private IMarkdownConverter _commonMarkConverter;
        private string _titleName = string.Empty;
        private Thickness _editorMargins;
        private FindReplaceDialog _findReplaceDialog;
        private IMarkdownConverter _githubMarkdownConverter;
        private bool _newVersion;
        private ISnippetManager _snippetManager;
        private ISpellCheckProvider _spellCheckProvider;
        private string _clock;
        private Visibility _clockIsVisible;

        public MainWindow MainWindow { get; set; }

        public string TitleName
        {
            get => _titleName;
            set => Set(ref _titleName, value);
        }

        public Thickness EditorMargins
        {
            get => _editorMargins;
            set => Set(ref _editorMargins, value);
        }

        public IMarkdownConverter CommonMarkConverter
        {
            get => _commonMarkConverter;
            set => Set(ref _commonMarkConverter, value);
        }

        public IMarkdownConverter GitHubMarkdownConverter
        {
            get => _githubMarkdownConverter;
            set => Set(ref _githubMarkdownConverter, value);
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get => _spellCheckProvider;
            set => Set(ref _spellCheckProvider, value);
        }

        public FindReplaceDialog FindReplaceDialog
        {
            get => _findReplaceDialog;
            set => Set(ref _findReplaceDialog, value);
        }

        public ISnippetManager SnippetManager
        {
            get => _snippetManager;
            set => Set(ref _snippetManager, value);
        }

        public bool NewVersion
        {
            get => _newVersion;
            set => Set(ref _newVersion, value);
        }

        public string Clock
        {
            get => _clock;
            set => Set(ref _clock, value);
        }

        public Visibility ClockIsVisible
        {
            get => _clockIsVisible;
            set => Set(ref _clockIsVisible, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            MainWindow.Editor.CloseHelp();
            MainWindow.DisplaySettings.SaveIfModified();
            cancelEventArgs.Cancel = !MainWindow.Editor.SaveIfModified();
            if (cancelEventArgs.Cancel || App.UserSettings.YesIDonated) return;
            var donate = new Donate { Owner = Application.Current.MainWindow };
            donate.ShowDialog();
        }

        public void OnFirstActivation(object sender, EventArgs eventArgs)
        {
            MainWindow.Activated -= OnFirstActivation;
            MainWindow.Dispatcher.InvokeAsync(async () =>
            {
                var updateMargins = Utility.Debounce(() =>
                    MainWindow.Dispatcher.Invoke(() => EditorMargins = CalculateEditorMargins()), 50);

                Func<Visibility> isVisible = () => MainWindow.WindowState == WindowState.Maximized
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                App.UserSettings.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == nameof(App.UserSettings.SinglePaneMargin)) updateMargins();
                    if (args.PropertyName == nameof(App.UserSettings.GitHubMarkdown)) MainWindow.Preview.UpdatePreview(MainWindow.Editor);
                };

                StartClock();
                ClockIsVisible = isVisible();
                MainWindow.SizeChanged += (s, e) => ClockIsVisible = isVisible();

                updateMargins();
                MainWindow.SizeChanged += (s, e) => updateMargins();

                LoadCommandLineOrLastFile();
                Application.Current.Activated += OnActivated;
                NewVersion = !await Version.IsCurrentVersion();
            });
        }

        public void OnActivated(object sender, EventArgs args)
        {
            if (MainWindow.Preview.Visibility == Visibility.Visible && MainWindow.Editor.Visibility != Visibility.Visible)
            {
                MainWindow.SetFocus(MainWindow.Preview.Browser);
            }
        }
        private void LoadCommandLineOrLastFile()
        {
            var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault()
                ?? (App.UserSettings.EditorOpenLastFile ? Settings.Default.LastOpenFile : null);

            if (string.IsNullOrWhiteSpace(fileToOpen)
                || fileToOpen == "-n"
                || !MainWindow.Editor.LoadFile(fileToOpen))
            {
                NewFileCommand.Command.Execute(null, MainWindow);
            }
        }

        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            MainWindow.IsVisibleChanged -= OnIsVisibleChanged;
            UpdateEditorPreviewVisibility(Settings.Default.EditPreviewHide);
            MainWindow.Activate();
        }

        public void UpdateEditorPreviewVisibility(int state)
        {
            MainWindow.Editor.Visibility = state == 2 ? Visibility.Collapsed : Visibility.Visible;
            MainWindow.PreviewAirspaceDecorator.Visibility = state == 1 ? Visibility.Collapsed : Visibility.Visible;
            MainWindow.Preview.Visibility = MainWindow.PreviewAirspaceDecorator.Visibility;
            MainWindow.UniformGrid.Columns = state == 0 ? 2 : 1;
            MainWindow.SetFocus(state == 2 ? MainWindow.Preview.Browser as IInputElement : MainWindow.Editor.EditBox);
            EditorMargins = CalculateEditorMargins();
        }

        private Thickness CalculateEditorMargins()
        {
            var singlePaneMargin = Math.Min(Math.Max(EditorMarginMin, App.UserSettings.SinglePaneMargin), EditorMarginMax);
            var margin = MainWindow.UniformGrid.Columns == 1 ? MainWindow.ActualWidth / singlePaneMargin : 0;
            return new Thickness(margin, 0, margin, 0);
        }

        public void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case nameof(Editor.FileName):
                case nameof(Editor.DisplayName):
                case nameof(Editor.IsModified):
                    TitleName = $"{App.Title} - {(MainWindow.Editor.IsModified ? "* " : "")}{MainWindow.Editor.DisplayName}";
                    break;
            }
        }

        private void StartClock()
        {
            Clock = DateTime.Now.ToShortTimeString();
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 15) };
            timer.Tick += (s, e) => Clock = DateTime.Now.ToShortTimeString();
            timer.Start();
        }
    }
}
