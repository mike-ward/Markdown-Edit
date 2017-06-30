using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class EditControlViewModel : BindableBase
    {
        public EditControlViewModel(ITextEditorComponent textEditor)
        {
            TextEditor = textEditor;
        }

        public ITextEditorComponent TextEditor { get; set; }

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

        private bool _wordWrap = true;
        public bool WordWrap
        {
            get => _wordWrap;
            set => SetProperty(ref _wordWrap, value);
        }
    }
}
