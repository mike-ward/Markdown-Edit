using System.Windows.Input;
using Infrastructure;
using MahApps.Metro.Controls;
using Prism.Events;

namespace UserModule.Views
{
    public class SettingsFlyout : Flyout
    {
        public SettingsFlyout(SettingsPanel panel, IEventAggregator eventAggregator)
        {
            Name = "SettingsFlyout";
            AnimateOpacity = true;
            Header = "Settings"; // todo: localize
            Position = Position.Right;
            Theme = FlyoutTheme.Accent;
            IsModal = true;
            Width = 400;
            Content = panel;

            KeyUp += (sender, args) => { if (args.Key == Key.Escape) IsOpen = false; };
            eventAggregator.GetEvent<DisplaySettingsEvent>().Subscribe(_ => IsOpen = !IsOpen);
        }
    }
}
