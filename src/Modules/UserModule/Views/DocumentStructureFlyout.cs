using MahApps.Metro.Controls;

namespace UserModule.Views
{
    public class DocumentStructureFlyout : Flyout
    {
        public DocumentStructureFlyout()
        {
            Name = "DocumentStructureFlyout";
            AnimateOpacity = true;
            Header = "Table of Contents"; // todo: localize
            Position = Position.Right;
            Theme = FlyoutTheme.Accent;
            IsModal = true;
            Width = 400;
        }
    }
}
