using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace MarkdownEdit
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static RoutedCommand WordWrapCommand = new RoutedUICommand();
        
        private string _titleName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
            Editor.PropertyChanged += EditorOnPropertyChanged;
            Editor.TextChanged += (s, e) => Preview.UpdatePreview(Editor.Text);
            Editor.ScrollChanged += (s, e) => Preview.SetScrollOffset(Convert.ToInt32(e.VerticalOffset));
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (Editor.IsModified == false) return;

            var result = MessageBox.Show(
                string.Format(@"Save ""{0}""?", Editor.FileName), 
                @"File Modified", 
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            switch (result)
            {
                case System.Windows.Forms.DialogResult.No:
                    break;

                case System.Windows.Forms.DialogResult.Yes:
                    Editor.SaveFile();
                    break;

                case System.Windows.Forms.DialogResult.Cancel:
                    cancelEventArgs.Cancel = true;
                    break;
            }
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            switch (ea.PropertyName)
            {
                case "FileName":
                case "DisplayName":
                case "IsModified":
                    TitleName = BuildTitle();
                    break;
            }
        }

        private string BuildTitle()
        {
            return string.Format("MARKDOWN EDIT - {0}{1}", Editor.IsModified ? "*" : "", Editor.DisplayName);
        }

        // Commands

        private void ExecuteNewFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.NewFile();
        }

        private void CanExecuteNewFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.OpenFile();
        }

        private void CanExecuteOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.CanExecute;
        }

        public void ExecuteSaveFile(object sender, ExecutedRoutedEventArgs ea)
        {
            Editor.SaveFile();
        }

        private void CanExecuteSaveFile(object sender, CanExecuteRoutedEventArgs e)
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

        // Properites

        public string TitleName
        {
            get { return _titleName; }
            set
            {
                if (_titleName == value) return;
                _titleName = value;
                OnPropertyChanged();
            }
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