using System;
using System.Collections.Generic;
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
        private Theme _theme;
        private readonly IEnumerable<IEditFeature> _editFeatures;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettings _settings;
        public TextEditor TextEditor { get; }
        public Dispatcher Dispatcher { get; set; }


        public EditControlViewModel(
            ITextEditorComponent textEditor,
            IEnumerable<IEditFeature> editFeatures,
            IEventAggregator eventAggregator,
            ISettings settings)
        {
            TextEditor = textEditor as TextEditor;
            _editFeatures = editFeatures;
            _eventAggregator = eventAggregator;
            _settings = settings;

            InitializeEditFeatures();
            EventHandlers();
            Theme = new Theme();
        }

        private void InitializeEditFeatures()
        {
            foreach (var editFeature in _editFeatures)
            {
                editFeature.Initialize(this);
            }
        }

        private void EventHandlers()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            // This ties changes from the Settings singleton to notifications to the UI
            // The editor and settings property name must be the same for this to work  
            _settings.PropertyChanged += (sd, ea) => RaisePropertyChanged(ea.PropertyName);

            TextEditor.Document.FileNameChanged += (sd, ea) => _eventAggregator
                .GetEvent<DocumentNameChangedEvent>()
                .Publish(TextEditor.Document.FileName);

            void UpdateTextCommand() => Dispatcher.InvokeAsync(
                () => _eventAggregator
                    .GetEvent<TextUpdatedEvent>()
                    .Publish(TextEditor.Document.Text));

            var debounceUpdateTextCommand = Utility.Debounce(UpdateTextCommand);
            TextEditor.Document.TextChanged += (sd, ea) => debounceUpdateTextCommand();
        }

        // Properties

        public FontFamily Font => _settings.Font;

        public double FontSize => _settings.FontSize;

        public bool WordWrap
        {
            get => _settings.WordWrap;
            set => _settings.WordWrap = value;
        }

        private bool _isDocumentModified;

        public bool IsDocumentModified
        {
            get => _isDocumentModified;
            set => SetProperty(ref _isDocumentModified, value, () => _eventAggregator.GetEvent<DocumentModifiedChangedEvent>().Publish(value));
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