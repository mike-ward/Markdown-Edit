using System;
using System.Windows.Media.Imaging;

namespace MarkdownEdit.Controls
{
    public partial class ImagePopup
    {
        public ImagePopup()
        {
            InitializeComponent();
            MouseDown += (sender, args) => IsOpen = false;
        }

        public void ShowImage(string uri)
        {
            _image.Source = new BitmapImage(new Uri(uri));
            IsOpen = true;
        }
    }
}