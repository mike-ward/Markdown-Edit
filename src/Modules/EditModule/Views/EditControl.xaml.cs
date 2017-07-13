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
            AddCommandBindings();
            AddEventHandlers(textEditor);
        }

        private void AddPropertyBindings(DependencyObject textEditor)
        {
            void AddBinding(DependencyProperty dp, string property, BindingMode mode = BindingMode.OneWay) 
                => BindingOperations.SetBinding(textEditor, dp, new Binding(property) { Source = DataContext, Mode = mode });

            AddBinding(FontFamilyProperty, nameof(ViewModel.Font));
            AddBinding(FontSizeProperty, nameof(ViewModel.FontSize));
            AddBinding(TextEditor.WordWrapProperty, nameof(ViewModel.WordWrap), BindingMode.TwoWay);
            AddBinding(TextEditor.IsModifiedProperty, nameof(ViewModel.IsDocumentModified), BindingMode.TwoWay);
        }

        private void AddCommandBindings()
        {
            var shell = (Window)RegionManager.Regions[Constants.EditRegion].Context;
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, ViewModel.NewCommandExecutedHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, ViewModel.OpenCommandExecuteHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, ViewModel.SaveCommandExecuteHandler));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, ViewModel.SaveAsCommandExecuteHandler));
        }

        private void AddEventHandlers(TextEditor textEditor)
        {
            IsVisibleChanged += (sd, ea) => { if (IsVisible) Dispatcher.InvokeAsync(textEditor.Focus); };
        }
    }
}
