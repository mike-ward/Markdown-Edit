using System;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;

namespace EditModule.Views
{
    public partial class EditControl
    {
        public EditControl()
        {
            InitializeComponent();

            var textEditor = (TextEditor)DataContext.GetType().GetProperty("TextEditor")?.GetValue(DataContext, null);
            if (textEditor == null) throw new NullReferenceException("TextEditor not created in view model");
            InitializeTextEditor(textEditor);
            _border.Child = textEditor;
            Dispatcher.InvokeAsync(() => textEditor.Focus());
        }

        private void InitializeTextEditor(DependencyObject textEditor)
        {
            void SetBinding(DependencyProperty dp, string property, BindingMode mode = BindingMode.OneWay)
            {
                BindingOperations.SetBinding(textEditor, dp, new Binding(property) { Source = DataContext, Mode = mode });
            }

            SetBinding(FontFamilyProperty, "Font");
            SetBinding(FontSizeProperty, "FontSize");
            SetBinding(TextEditor.WordWrapProperty, "WordWrap", BindingMode.TwoWay);
        }

    }
}
