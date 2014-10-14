using System;
using System.ComponentModel;
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
        public static RoutedCommand SetTitleFileNameCommand = new RoutedUICommand();

        private string _titleFileName;

        public MainWindow()
        {
            InitializeComponent();
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
            Editor.WordWrapHandler();
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

        public void ExecuteSetTitleFileName(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            TitleFileName = ea.Parameter as string;
        }

        public string TitleFileName
        {
            get { return "Markdown Edit - " + (_titleFileName ?? "Press F1 for Help"); }
            set
            {
                if (_titleFileName != value)
                {
                    _titleFileName = value;
                    OnPropertyChanged();                    
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}