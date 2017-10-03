using System.Windows;
using System.Windows.Input;

namespace UserModule.Commands
{
    public interface IUserCommandHandler
    {
        void Initialize(UIElement uiElement);
        void Execute(object sender, ExecutedRoutedEventArgs ea);

    }
}