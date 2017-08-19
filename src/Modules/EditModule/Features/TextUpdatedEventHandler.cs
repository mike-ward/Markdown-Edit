using EditModule.ViewModels;
using Infrastructure;
using Prism.Events;

namespace EditModule.Features
{
    public class TextUpdatedEventHandler : IEditFeature
    {
        private readonly IEventAggregator _eventAggregator;

        public TextUpdatedEventHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize(EditControlViewModel viewModel)
        {

            void UpdateTextAction() => viewModel.Dispatcher.InvokeAsync(
                () => _eventAggregator
                    .GetEvent<TextUpdatedEvent>()
                    .Publish(viewModel.TextEditor.Document.Text));

            var debounceUpdateTextAction = Utility.Debounce(UpdateTextAction);
            viewModel.TextEditor.Document.TextChanged += (sd, ea) => debounceUpdateTextAction();
        }
    }
}
