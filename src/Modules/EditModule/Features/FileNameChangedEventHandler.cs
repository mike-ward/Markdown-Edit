using EditModule.ViewModels;
using Infrastructure;
using Prism.Events;

namespace EditModule.Features
{
    public class FileNameChangedEventHandler : IEditFeature
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettings _settings;

        public FileNameChangedEventHandler(IEventAggregator eventAggregator, ISettings settings)
        {
            _eventAggregator = eventAggregator;
            _settings = settings;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            viewModel.TextEditor.Document.FileNameChanged += (sd, ea) => _eventAggregator
                .GetEvent<DocumentNameChangedEvent>()
                .Publish(viewModel.TextEditor.Document.FileName);

            viewModel.TextEditor.Document.FileNameChanged += (sd, ea) => _settings
                .CurrentFileName = viewModel.TextEditor.Document.FileName;
        }
    }
}
