using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EditModule.Models;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Events;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class EditControlViewModel : BindableBase
    {
        public TextEditor TextEditor { get; set; }
        public IEditModel EditModel { get; }
        public IMarkdownEngine MarkdownEngine { get; }
        public IEventAggregator EventAggregator { get; }
        public IOpenSaveActions OpenSaveActions { get; }
        public ISettings Settings { get; }
        public INotify Notify { get; }
        public Dispatcher Dispatcher { get; set; }

        public EditControlViewModel(
            IEditModel editModel,
            ITextEditorComponent textEditor,
            IMarkdownEngine markdownEngine,
            IEventAggregator eventAggregator,
            IOpenSaveActions openSaveActions,
            ISettings settings,
            INotify notify)
        {
            EditModel = editModel;
            TextEditor = textEditor as TextEditor;
            MarkdownEngine = markdownEngine;
            EventAggregator = eventAggregator;
            OpenSaveActions = openSaveActions;
            Settings = settings;
            Notify = notify;

            SetTextEditorOptions();
            AddEventHandlers();
        }

        private void SetTextEditorOptions()
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

            TextEditor.Document.FileNameChanged += (sd, ea) => EventAggregator.GetEvent<DocumentNameChangedEvent>().Publish(TextEditor.Document.FileName);

            void ExecuteUpdateTextCommand() => Dispatcher.InvokeAsync(() => EventAggregator.GetEvent<TextUpdatedEvent>().Publish(TextEditor.Document.Text));
            var debounceUpdateTextCommand = Utility.Debounce(ExecuteUpdateTextCommand);
            TextEditor.Document.TextChanged += (sd, ea) => debounceUpdateTextCommand();
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
            EditModel.NewCommandHandler(TextEditor);
        }

        public void OpenCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            EditModel.OpenCommandHandler(TextEditor, ea.Parameter as string);
        }

        public void SaveCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
           EditModel.SaveCommandHandler(TextEditor);
        }

        public void SaveAsCommandExecuteHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            EditModel.SaveAsCommandHandler(TextEditor);
        }
    }
}