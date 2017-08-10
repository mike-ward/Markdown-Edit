using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EditModule.Features;
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
        public IEventAggregator EventAggregator { get; }
        public IOpenSaveActions OpenSaveActions { get; }
        public ISettings Settings { get; }
        public Dispatcher Dispatcher { get; set; }

        private Theme _theme;

        public EditControlViewModel(
            IEditModel editModel,
            ITextEditorComponent textEditor,
            IEventAggregator eventAggregator,
            IOpenSaveActions openSaveActions,
            ISettings settings,
            IEnumerable<IEditFeature> editFeatures)
        {
            EditModel = editModel;
            TextEditor = textEditor as TextEditor;
            EventAggregator = eventAggregator;
            OpenSaveActions = openSaveActions;
            Settings = settings;

            foreach (var editFeature in editFeatures)
            {
                editFeature.Initialize(this);
            }

            EventHandlers();
            Theme = new Theme();
        }

        private void EventHandlers()
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

        // Theme

        public EventHandler<ThemeChangedEventArgs> ThemeChanged;

        public Theme Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value, () => ThemeChanged?.Invoke(this, new ThemeChangedEventArgs {Theme = value}));
        }
    }
}