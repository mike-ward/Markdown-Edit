using EditModule.ViewModels;

namespace EditModule.Features
{
    public interface IEditFeature
    {
        void Initialize(EditControlViewModel viewModel);
    }
}