using System;
using System.Windows.Media;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class EditControlViewModel : BindableBase
    {
        private FontFamily _fontFamily = new FontFamily("Consolas");

        public FontFamily Font
        {
            get => _fontFamily;
            set => SetProperty(ref _fontFamily, value);
        }

        private double _fontSize = 16;

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }
    }
}
