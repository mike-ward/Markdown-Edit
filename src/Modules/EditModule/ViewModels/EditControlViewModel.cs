using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class EditControlViewModel : BindableBase
    {
        public TextEditor TextEditor { get; set; }
        public IMarkdownEngine MarkdownEngine { get; }
        public IEventAggregator EventAggregator { get; }
        public IOpenSaveActions OpenSaveActions { get; }
        public ISettings Settings { get; }
        public INotify Notify { get; }
        public Dispatcher Dispatcher { get; set; }

        public DelegateCommand<string> UpdateTextCommand { get; set; }

        public EditControlViewModel(
            ITextEditorComponent textEditor,
            IMarkdownEngine markdownEngine,
            IEventAggregator eventAggregator,
            IOpenSaveActions openSaveActions,
            ISettings settings,
            INotify notify)
        {
            TextEditor = textEditor as TextEditor;
            MarkdownEngine = markdownEngine;
            EventAggregator = eventAggregator;
            OpenSaveActions = openSaveActions;
            Settings = settings;
            Notify = notify;

            TextEditorOptions();
            AddEventHandlers();
            InstantiateCommands();
        }

        private void TextEditorOptions()
        {
            var options = TextEditor.Options;
            options.IndentationSize = 2;
            options.AllowToggleOverstrikeMode = true;
            options.EnableHyperlinks = false;
            options.EnableEmailHyperlinks = false;
            options.CutCopyWholeLine = true;
            options.ConvertTabsToSpaces = true;
            options.AllowScrollBelowDocument = true;
            options.EnableRectangularSelection = true;
        }

        private void AddEventHandlers()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            // This ties changes from the Settings singleton to notifications to the UI
            Settings.PropertyChanged += (sd, ea) => RaisePropertyChanged(ea.PropertyName);

            void ExecuteUpdateTextCommand() => Dispatcher.InvokeAsync(() => UpdateTextCommand.Execute(TextEditor.Document.Text));
            var debounceUpdateTextCommand = Utility.Debounce(ExecuteUpdateTextCommand);
            TextEditor.Document.TextChanged += (sd, ea) => debounceUpdateTextCommand();
            TextEditor.Document.FileNameChanged += (sd, ea) => EventAggregator.GetEvent<DocumentNameChangedEvent>().Publish(TextEditor.Document.FileName);
        }

        private void InstantiateCommands()
        {
            UpdateTextCommand = new DelegateCommand<string>(text => EventAggregator.GetEvent<TextUpdatedEvent>().Publish(text));
        }

        public FontFamily Font => Settings.Font;

        public double FontSize => Settings.FontSize;

        public bool WordWrap
        {
            get => Settings.WordWrap;
            set => Settings.WordWrap = value;
        }

        private bool _isDocumentModified;

        public bool IsDocumentModified
        {
            get => _isDocumentModified;
            set => SetProperty(ref _isDocumentModified, value, () => EventAggregator.GetEvent<DocumentModifiedChangedEvent>().Publish(value));
        }

        public void NewCommandExecutedHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            if (TextEditor.IsModified)
            {
                if (OpenSaveActions.PromptToSave(TextEditor.Document.FileName, TextEditor.Text) == MessageBoxResult.Cancel) return;
            }
            TextEditor.Document.Text = string.Empty;
            TextEditor.Document.FileName = string.Empty;
            TextEditor.IsModified = false;
        }

        public void OpenCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            if (TextEditor.IsModified)
            {
                if (OpenSaveActions.PromptToSave(TextEditor.Document.FileName, TextEditor.Text) == MessageBoxResult.Cancel) return;
            }

            var file = ea.Parameter as string ?? OpenSaveActions.OpenDialog();

            try
            {
                var text = OpenSaveActions.Open(file);
                TextEditor.Document.Text = text;
                TextEditor.Document.FileName = file;
                TextEditor.ScrollToHome();
                TextEditor.IsModified = false;

            }
            catch (Exception ex)
            {
                Notify.Alert(ex.Message);
            }
        }

        public void SaveCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                if (string.IsNullOrEmpty(TextEditor.Document.FileName))
                {
                    SaveAsCommandExecuteHandler(sender, ea);
                    return;
                }
                OpenSaveActions.Save(TextEditor.Document.FileName, TextEditor.Document.Text);
                TextEditor.IsModified = false;
            }
            catch (Exception ex)
            {
                Notify.Alert(ex.Message);
            }
        }

        public void SaveAsCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var fileName = OpenSaveActions.SaveAsDialog();
            if (string.IsNullOrEmpty(fileName)) return;
            TextEditor.Document.FileName = fileName;
            SaveCommandExecuteHandler(sender, ea);
        }
    }
}