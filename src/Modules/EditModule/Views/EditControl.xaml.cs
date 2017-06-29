using System.Windows.Data;
using ICSharpCode.AvalonEdit;

namespace EditModule.Views
{
    public partial class EditControl
    {
        public EditControl()
        {
            InitializeComponent();
            var textEditor = InitializeTextEditor();
            _border.Child = textEditor;
            Dispatcher.InvokeAsync(() => textEditor.Focus());
        }

        private TextEditor InitializeTextEditor()
        {
            var textEditor = new TextEditor();
            textEditor.SetBinding(FontFamilyProperty, new Binding("Font") {Source = DataContext, Mode = BindingMode.OneWay});
            textEditor.SetBinding(FontSizeProperty, new Binding("FontSize") {Source = DataContext, Mode = BindingMode.OneWay});
            return textEditor;
        }
    }
}
