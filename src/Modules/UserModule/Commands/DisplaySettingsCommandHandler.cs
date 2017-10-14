using System.Windows;
using System.Windows.Input;
using Infrastructure;
using Prism.Events;

namespace UserModule.Commands
{
    public class DisplaySettingsCommandHandler : IUserCommandHandler
    {
        private readonly IEventAggregator _eventAggregator;
        public static readonly RoutedCommand Command = new RoutedCommand();

        public DisplaySettingsCommandHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Initialize(UIElement uiElement)
        {
            uiElement.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _eventAggregator.GetEvent<DisplaySettingsEvent>().Publish(true);
        }
    }
}
