using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
        public bool CanExecute { get; private set; }
        private string _fileName;
        private string _displayName = string.Empty;
        private bool _wordWrap = true;
        private bool _isModified;
        private EditorState _editorState = new EditorState();

        private struct EditorState
        {
            public bool IsSaved { get; private set; }
            private string _text;
            private bool _isModified;
            private bool _wordWrap;
            private bool _canExecute;

            public void Save(Editor editor)
            {
                _text = editor.Text;
                _isModified = editor.IsModified;
                _wordWrap = editor.WordWrap;
                _canExecute = editor.CanExecute;
                editor.IsModified = false;
                editor.WordWrap = true;
                editor.IsReadOnly = true;
                IsSaved = true;
                editor.CanExecute = false;
            }

            public void Restore(Editor editor)
            {
                editor.Text = _text;
                editor.IsModified = _isModified;
                editor.WordWrap = _wordWrap;
                editor.IsReadOnly = false;
                editor.CanExecute = _canExecute;
                editor.DisplayName = string.Empty;
                IsSaved = false;
            }
        }

        public Editor()
        {
            InitializeComponent();
            CanExecute = true;
            EditBox.Loaded += EditBoxOnLoaded;
        }

        private void EditBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var reader = new XmlTextReader(new StringReader(Properties.Resources.markdown_xshd));
            var xshd = HighlightingLoader.LoadXshd(reader);
            var highlighter = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            EditBox.SyntaxHighlighting = highlighter;
            reader.Close();

            EditBox.Options.IndentationSize = 2;
            EditBox.Options.ConvertTabsToSpaces = true;
            EditBox.Options.AllowScrollBelowDocument = true;
            EditBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            Dispatcher.InvokeAsync(() =>
            {
                EditBox.TextChanged += EditBoxOnTextChanged;
                LoadFile(Settings.Default.LastOpenFile);
                EditBox.Focus();
            });
        }

        // Commands

        public void OpenFile()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadFile(dialog.FileNames[0]);
        }

        private void LoadFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            EditBox.Text = File.ReadAllText(file);
            Settings.Default.LastOpenFile = file;
            IsModified = false;
            FileName = file;
        }

        public void ToggleHelp()
        {
            if (_editorState.IsSaved)
            {
                _editorState.Restore(this);
                return;
            }
            _editorState.Save(this);
            Text = Properties.Resources.Help;
            EditBox.IsModified = false;
            DisplayName = "Help";
        }

        // Events

        public EventHandler TextChanged;

        private void EditBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            var handler = TextChanged;
            if (handler != null) TextChanged(this, eventArgs);
        }

        public event ScrollChangedEventHandler ScrollChanged;

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            var handler = ScrollChanged;
            if (handler != null) handler(this, scrollChangedEventArgs);
        }

        // Properties 

        public string Text
        {
            get { return EditBox.Text; }
            set { EditBox.Text = value; }
        }

        public bool IsReadOnly
        {
            get { return EditBox.IsReadOnly; }
            set { EditBox.IsReadOnly = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get
            {
                return string.IsNullOrWhiteSpace(_displayName)
                    ? Path.GetFileName(FileName)
                    : _displayName;
            }
            set
            {
                if (_displayName == value) return;
                _displayName = value;
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

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}