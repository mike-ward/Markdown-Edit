using MahApps.Metro.Controls;

namespace UserModule.Views
{
    public class SettingsFlyout : Flyout
    {
        public SettingsFlyout(SettingsPanel panel)
        {
            Name = "SettingsFlyout";
            AnimateOpacity = true;
            Header = "Settings";
            Position = Position.Right;
            Theme = FlyoutTheme.Accent;
            IsModal = true;
            Width = 400;
            Content = panel;
        }
    }
}
