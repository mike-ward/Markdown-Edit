using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class Editor : INotifyPropertyChanged
    {
        private string _filename;
        private string _textSwap;
        private bool _wordWrap = true;
        private bool _isModified;

        public Editor()
        {
            InitializeComponent();
            EditorBox.Loaded += EditorBoxOnLoaded;
        }

        private void EditorBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var reader = new XmlTextReader(new StringReader(Properties.Resources.markdown_xshd));
            var xshd = HighlightingLoader.LoadXshd(reader);
            var highlighter = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            EditorBox.SyntaxHighlighting = highlighter;
            reader.Close();

            EditorBox.Options.IndentationSize = 2;
            EditorBox.Options.ConvertTabsToSpaces = true;
            EditorBox.Options.AllowScrollBelowDocument = true;
            EditorBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            Dispatcher.InvokeAsync(() =>
            {
                EditorBox.TextChanged += EditorBoxOnTextChanged;
                LoadFile(Settings.Default.LastOpenFile);
                EditorBox.Focus();
            });
        }

        private void EditorBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            MainWindow.UpdatePreviewCommand.Execute(EditorBox.Text, this);
        }

        public void OpenFileHandler()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadFile(dialog.FileNames[0]);
        }

        private void LoadFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            EditorBox.Text = File.ReadAllText(file);
            FileLoaded(file);
        }

        private void FileLoaded(string file)
        {
            Settings.Default.LastOpenFile = file;
            Filename = file;
        }

        public void ToggleHelp()
        {
            if (_textSwap == null)
            {
                _textSwap = EditorBox.Text;
                EditorBox.Text = Properties.Resources.Help;
                EditorBox.IsReadOnly = true;
            }
            else
            {
                EditorBox.IsReadOnly = false;
                EditorBox.Text = _textSwap;
                _textSwap = null;
            }
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            MainWindow.ScrollPreviewCommand.Execute(EditorBox.VerticalOffset, this);
        }

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename == value) return;
                _filename = value;
                OnPropertyChanged();
            }
        }
        public bool WordWrap
        {
            get { return _wordWrap; }
            set
            {
                if (_wordWrap == value) return;
                _wordWrap = value;
                OnPropertyChanged();
            }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                if (_isModified == value) return;
                _isModified = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}