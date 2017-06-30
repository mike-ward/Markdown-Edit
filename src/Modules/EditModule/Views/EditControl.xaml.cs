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
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var textEditor = (TextEditor)DataContext.GetType().GetProperty("TextEditor")?.GetValue(DataContext, null);
            _border.Child = textEditor ?? throw new NullReferenceException("TextEditor not created in view model");

            AddBindings(textEditor);
            AddEventHandlers(textEditor);
        }

        private void AddBindings(DependencyObject textEditor)
        {
            void SetBinding(DependencyProperty dp, string property, BindingMode mode = BindingMode.OneWay) 
                => BindingOperations.SetBinding(textEditor, dp, new Binding(property) { Source = DataContext, Mode = mode });

            SetBinding(FontFamilyProperty, "Font");
            SetBinding(FontSizeProperty, "FontSize");
            SetBinding(TextEditor.WordWrapProperty, "WordWrap", BindingMode.TwoWay);
        }

        private void AddEventHandlers(TextEditor textEditor)
        {
            IsVisibleChanged += (sd, ea) => { if (IsVisible) Dispatcher.InvokeAsync(textEditor.Focus); };
        }
    }
}
