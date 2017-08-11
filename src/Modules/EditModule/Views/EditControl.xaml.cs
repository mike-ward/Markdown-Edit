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
        public EditControlViewModel ViewModel => (EditControlViewModel)DataContext;

        public EditControl(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _border.Child = ViewModel.TextEditor;
            Background = ViewModel.TextEditor.Background;
            ViewModel.Dispatcher = Dispatcher;

            AddPropertyBindings(ViewModel.TextEditor);
            AddCommandBindings();
            AddEventHandlers(ViewModel.TextEditor);
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
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, ViewModel.NewCommandExecuted));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, ViewModel.OpenCommandExecuted));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, ViewModel.SaveCommandExecuted));
            shell.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, ViewModel.SaveAsCommandExecuted));
        }

        private void AddEventHandlers(TextEditor textEditor)
        {
            IsVisibleChanged += (sd, ea) => { if (IsVisible) Dispatcher.InvokeAsync(textEditor.Focus); };
            ViewModel.ThemeChanged += (sd, ea) => Dispatcher.InvokeAsync(() => Background = ViewModel.TextEditor.Background);
        }

        protected override void OnDragEnter(DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop) == false) dea.Effects = DragDropEffects.None;
        }

        protected override void OnDrop(DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = dea.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null) return;
                Dispatcher.InvokeAsync(() => ApplicationCommands.Open.Execute(files[0], this));
            }
        }
    }
}
