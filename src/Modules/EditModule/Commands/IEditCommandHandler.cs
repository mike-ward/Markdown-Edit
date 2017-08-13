using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;

namespace EditModule.Commands
{
    public interface IEditCommandHandler
    {
        void Initialize(UIElement uiElement, EditControlViewModel viewModel);
        void Execute(object sender, ExecutedRoutedEventArgs ea);
    }
}