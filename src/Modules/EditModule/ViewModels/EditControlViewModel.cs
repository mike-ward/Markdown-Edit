using System.Windows.Media;
using System.Windows.Threading;
using EditModule.Commands;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class EditControlViewModel : BindableBase
    {
        public ITextEditorComponent TextEditor { get; set; }
        public IMarkdownEngine MarkdownEngine { get; }
        public IEventAggregator EventAggregator { get; }
        public IOpenSaveActions OpenSaveActions { get; }
        public ISettings Settings { get; }
        public INotify Notify { get; }
        public Dispatcher Dispatcher { get; set; }

        public DelegateCommand<string> UpdateTextCommand { get; set; }
        public OpenCommand OpenCommand { get; private set; }
        public OpenDialogCommand OpenDialogCommand { get; private set; }
        public SaveCommand SaveCommand { get; private set; }

        public EditControlViewModel(
            ITextEditorComponent textEditor,
            IMarkdownEngine markdownEngine,
            IEventAggregator eventAggregator,
            IOpenSaveActions openSaveActions,
            ISettings settings,
            INotify notify)
        {
            TextEditor = textEditor;
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
            TextEditor.Document.TextChanged += (sd, ea) => IsDocumentModified = true;
            TextEditor.Document.FileNameChanged += (sd, ea) => EventAggregator.GetEvent<FileNameChangedEvent>().Publish(TextEditor.Document.FileName);
        }

        private void InstantiateCommands()
        {
            UpdateTextCommand = new DelegateCommand<string>(text => EventAggregator.GetEvent<TextUpdatedEvent>().Publish(text));
            OpenCommand = new OpenCommand(this, OpenSaveActions, Notify);
            OpenDialogCommand = new OpenDialogCommand(OpenCommand, OpenSaveActions);
            SaveCommand = new SaveCommand(this, OpenSaveActions, Notify);
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
    }
}