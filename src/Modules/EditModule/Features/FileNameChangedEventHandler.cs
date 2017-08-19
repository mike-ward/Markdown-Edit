using EditModule.ViewModels;
using Infrastructure;
using Prism.Events;

namespace EditModule.Features
{
    public class FileNameChangedEventHandler : IEditFeature
    {
        private readonly IEventAggregator _eventAggregator;

        public FileNameChangedEventHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            viewModel.TextEditor.Document.FileNameChanged += (sd, ea) => _eventAggregator
                .GetEvent<DocumentNameChangedEvent>()
                .Publish(viewModel.TextEditor.Document.FileName);
        }
    }
}
