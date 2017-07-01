using System;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Commands;

namespace EditModule.Views
{
    public partial class EditControl
    {
        public EditControl()
        {
            InitializeComponent();
        }

        private T GetViewModelProperty<T>(string name) => (T)DataContext.GetType().GetProperty(name)?.GetValue(DataContext, null);

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var textEditor =  GetViewModelProperty<TextEditor>("TextEditor");
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

            // Debounce text updates for performance
            var updateTextCommand = GetViewModelProperty<DelegateCommand<string>>("UpdateTextCommand");
            void ExecuteUpdateTextCommand() => Dispatcher.InvokeAsync(() => updateTextCommand.Execute(textEditor.Text));
            var executeUpdateTextCommand = Utility.Debounce(ExecuteUpdateTextCommand);
            textEditor.TextChanged += (sd, ea) => executeUpdateTextCommand();
        }
    }
}
