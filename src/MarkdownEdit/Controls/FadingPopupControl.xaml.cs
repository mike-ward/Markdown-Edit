using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MarkdownEdit.Controls
{
    public partial class FadingPopupControl
    {
        private Window _parentWindow;
        private IInputElement _focusedElement;

        public FadingPopupControl()
        {
            InitializeComponent();
        }

        public void ShowDialogBox(Window parentWindow, string message, double width = 400, double height = 100)
        {
            _parentWindow = parentWindow;
            _focusedElement = Keyboard.FocusedElement;

            Width = width;
            Height = height;
            Popup.Width = width;
            Popup.Height = height;
            Popup.PlacementTarget = _parentWindow;
            PopupLabel.Content = message;
            Popup.IsOpen = true;

            var statusFader = (Storyboard)Resources["StatusFader"];
            statusFader.Begin(PopupBackground);
        }

        private void StatusFader_Completed(object sender, EventArgs e)
        {
            Popup.IsOpen = false;
            if (_focusedElement != null) Keyboard.Focus(_focusedElement);
        }
    }
}