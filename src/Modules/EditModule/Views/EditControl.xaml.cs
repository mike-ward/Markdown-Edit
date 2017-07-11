using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Regions;

namespace EditModule.Views
{
    public partial class EditControl
    {
        public IRegionManager RegionManager { get; }

        public EditControl(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            InitializeComponent();
        }

        private EditControlViewModel ViewModel => (EditControlViewModel)DataContext;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var textEditor = ViewModel.TextEditor;
            _border.Child = textEditor ?? throw new NullReferenceException("TextEditor not created in view model");

            ViewModel.Dispatcher = Dispatcher;
            AddPropertyBindings(textEditor);
            AddEventHandlers(textEditor);
            AddCommandBindings();
            AddKeyboardBindings();
        }

        private void AddPropertyBindings(DependencyObject textEditor)
        {
            void AddBinding(DependencyProperty dp, string property, BindingMode mode = BindingMode.OneWay) 
                => BindingOperations.SetBinding(textEditor, dp, new Binding(property) { Source = DataContext, Mode = mode });

            AddBinding(FontFamilyProperty, "Font");
            AddBinding(FontSizeProperty, "FontSize");
            AddBinding(TextEditor.WordWrapProperty, "WordWrap", BindingMode.TwoWay);
            AddBinding(TextEditor.IsModifiedProperty, "IsDocumentModified", BindingMode.TwoWay);
        }

        private void AddEventHandlers(TextEditor textEditor)
        {
            IsVisibleChanged += (sd, ea) => { if (IsVisible) Dispatcher.InvokeAsync(textEditor.Focus); };
        }

        private void AddCommandBindings()
        {
            var shell = (Window)RegionManager.Regions[Constants.EditRegion].Context;
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, ViewModel.NewCommandExecutedHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, ViewModel.OpenCommandExecuteHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, ViewModel.SaveCommandExecuteHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, ViewModel.SaveAsCommandExecuteHandler));
        }

        private void AddKeyboardBindings()
        {
            var shell = (Window)RegionManager.Regions[Constants.EditRegion].Context;

            shell.InputBindings.Add(new KeyBinding {Key = Key.N, Modifiers = ModifierKeys.Control, Command = ApplicationCommands.New});
            shell.InputBindings.Add(new KeyBinding {Key = Key.O, Modifiers = ModifierKeys.Control, Command = ApplicationCommands.Open});
            shell.InputBindings.Add(new KeyBinding {Key = Key.S, Modifiers = ModifierKeys.Control, Command = ApplicationCommands.Save});
            shell.InputBindings.Add(new KeyBinding {Key = Key.S, Modifiers = ModifierKeys.Control | ModifierKeys.Shift, Command = ApplicationCommands.SaveAs});
        }
    }
}
