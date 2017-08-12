using System.Windows.Input;
using EditModule.ViewModels;

namespace EditModule.Commands
{
    public interface IEditCommandHandler
    {
        string Name { get; }
        void Initialize(EditControlViewModel viewModel);
        void Execute(object sender, ExecutedRoutedEventArgs ea);
    }
}