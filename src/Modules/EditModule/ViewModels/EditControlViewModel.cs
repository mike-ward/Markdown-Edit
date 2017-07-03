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
        public IFileActions FileActions { get; }
        public Dispatcher Dispatcher { get; set; }

        public DelegateCommand<string> UpdateTextCommand { get; set; }
        public OpenCommand OpenCommand { get; private set; }
        public OpenDialogCommand OpenDialogCommand { get; private set; }

        public EditControlViewModel(
            ITextEditorComponent textEditor,
            IMarkdownEngine markdownEngine,
            IEventAggregator eventAggregator,
            IFileActions fileActions)
        {
            TextEditor = textEditor;
            MarkdownEngine = markdownEngine;
            EventAggregator = eventAggregator;
            FileActions = fileActions;

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
            void ExecuteUpdateTextCommand() => Dispatcher.InvokeAsync(() => UpdateTextCommand.Execute(TextEditor.Document.Text));
            var debounceUpdateTextCommand = Utility.Debounce(ExecuteUpdateTextCommand);
            TextEditor.Document.TextChanged += (sd, ea) => debounceUpdateTextCommand();
            TextEditor.Document.FileNameChanged += (sd, ea) => EventAggregator.GetEvent<FileNameChangedEvent>().Publish(TextEditor.Document.FileName);
        }

        private void InstantiateCommands()
        {
            UpdateTextCommand = new DelegateCommand<string>(text => EventAggregator.GetEvent<TextUpdatedEvent>().Publish(text));
            OpenCommand = new OpenCommand(TextEditor, FileActions);
            OpenDialogCommand = new OpenDialogCommand(OpenCommand, FileActions);
        }

        private FontFamily _fontFamily = new FontFamily("Consolas");

        public FontFamily Font
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        private double _fontSize = 16;

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private bool _wordWrap = true;

        public bool WordWrap
        {
            get => _wordWrap;
            set => SetProperty(ref _wordWrap, value);
        }
    }
}