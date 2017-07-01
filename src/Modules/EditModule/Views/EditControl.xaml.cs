using System;
using System.Windows;
using System.Windows.Data;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Views
{
    public partial class EditControl
    {
        public EditControl()
        {
            InitializeComponent();
        }

        private EditControlViewModel ViewModel => (EditControlViewModel)DataContext;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var textEditor = ViewModel.TextEditor as TextEditor;
            _border.Child = textEditor ?? throw new NullReferenceException("TextEditor not created in view model");

            AddBindings(textEditor);
            AddEventHandlers(textEditor);
        }

        private void AddBindings(DependencyObject textEditor)
        {
            void AddBinding(DependencyProperty dp, string property, BindingMode mode = BindingMode.OneWay) 
                => BindingOperations.SetBinding(textEditor, dp, new Binding(property) { Source = DataContext, Mode = mode });

            AddBinding(FontFamilyProperty, "Font");
            AddBinding(FontSizeProperty, "FontSize");
            AddBinding(TextEditor.WordWrapProperty, "WordWrap", BindingMode.TwoWay);
        }

        private void AddEventHandlers(TextEditor textEditor)
        {
            IsVisibleChanged += (sd, ea) => { if (IsVisible) Dispatcher.InvokeAsync(textEditor.Focus); };

            var executeUpdateTextCommand = UpdateTextCommandAction(textEditor);
            textEditor.TextChanged += (sd, ea) => executeUpdateTextCommand();
        }

        private Action UpdateTextCommandAction(TextEditor textEditor)
        {
            var updateTextCommand = ViewModel.UpdateTextCommand;
            void ExecuteUpdateTextCommand() => Dispatcher.InvokeAsync(() => updateTextCommand.Execute(textEditor.Text));
            return Utility.Debounce(ExecuteUpdateTextCommand);
        }
    }
}
