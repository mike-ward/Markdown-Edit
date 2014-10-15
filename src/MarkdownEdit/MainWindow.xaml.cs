using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand UpdatePreviewCommand = new RoutedUICommand();
        public static RoutedCommand WordWrapCommand = new RoutedUICommand();
        public static RoutedCommand ToggleHelpCommand = new RoutedUICommand();
        public static RoutedCommand ScrollPreviewCommand = new RoutedUICommand();

        public MainWindow()
        {
            InitializeComponent();
            Editor.PropertyChanged += EditorOnPropertyChanged;
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case "Filename":
                    TitleName = null;
                    break;
            }
        }

        private void ExecuteUpdatePreview(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Preview.UpdatePreview(ea.Parameter as string);
        }

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Editor.OpenFileHandler();
        }

        public void ExecuteWordWrap(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Editor.WordWrap = !Editor.WordWrap;
        }

        public void ExecuteToggleHelp(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Editor.ToggleHelp();
        }

        public void ExecuteScrollPreview(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Preview.SetScrollOffset(Convert.ToInt32(ea.Parameter));
        }

        public string TitleName
        {
            get { return string.Format("MARKDOWN EDIT - {0}", Path.GetFileName(Editor.Filename) ?? "Press F1 for Help"); }
            // ReSharper disable once ValueParameterNotUsed
            set { OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}