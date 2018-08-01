using EditModule.ViewModels;
using Infrastructure;
using Prism.Events;

namespace EditModule.Features
{
    public class GetFocusOnFlyoutClosed : IEditFeature
    {
        private readonly IEventAggregator _eventAggregator;

        public GetFocusOnFlyoutClosed(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            _eventAggregator.GetEvent<FlyoutClosedEvent>().Subscribe(_ => viewModel.TextEditor.Focus(), true);
        }
    }
}
