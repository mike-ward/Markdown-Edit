using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand WordWrapCommand = new RoutedUICommand();

        public MainWindow()
        {
            InitializeComponent();
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(Convert.ToInt32(e.VerticalOffset));
            Editor.TextChanged += (s, e) => Preview.UpdatePreview(Editor.Text);
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case "FileName":
                case "DisplayName":
                case "IsModified":
                    TitleName = null;
                    break;
            }
        }

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.OpenFile();
        }

        private void CanExecuteOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        public void ExecuteWordWrap(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.WordWrap = !Editor.WordWrap;
        }

        public void ExecuteHelp(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.ToggleHelp();
        }

        public string TitleName
        {
            get { return string.Format("MARKDOWN EDIT - {0}{1}", Editor.IsModified ? "*" : "", Editor.DisplayName); }
            // ReSharper disable once ValueParameterNotUsed
            set { OnPropertyChanged(); }
        }

        // INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}