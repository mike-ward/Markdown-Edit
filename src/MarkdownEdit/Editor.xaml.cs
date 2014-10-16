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
        private HelpSwap _helpSwap = new HelpSwap();

        private struct HelpSwap
        {
            public bool IsSet { get; private set; }
            private bool _isModified;
            private bool _wordWrap;
            private string _text;

            public void Set(Editor editor)
            {
                _text = editor.Text;
                _isModified = editor.IsModified;
                _wordWrap = editor.WordWrap;
                editor.IsModified = false;
                editor.WordWrap = true;
                editor.IsReadOnly = true;
                IsSet = true;
                editor.CanExecute = false;
            }

            public void Reset(Editor editor)
            {
                editor.Text = _text;
                editor.IsModified = _isModified;
                editor.WordWrap = _wordWrap;
                editor.IsReadOnly = false;
                editor.DisplayName = string.Empty;
                IsSet = false;
                editor.CanExecute = true;
            }
        }

        public Editor()
        {
            InitializeComponent();
            CanExecute = true;
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
            EditorBox.Text = File.ReadAllText(file);
            Settings.Default.LastOpenFile = file;
            IsModified = false;
            FileName = file;
        }

        public void ToggleHelp()
        {
            if (_helpSwap.IsSet)
            {
                _helpSwap.Reset(this);
                return;
            }
            _helpSwap.Set(this);
            Text = Properties.Resources.Help;
            EditorBox.IsModified = false;
            DisplayName = "Help";
        }

        // Events

        public EventHandler TextChanged;

        private void EditorBoxOnTextChanged(object sender, EventArgs eventArgs)
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
            get { return EditorBox.Text; }
            set { EditorBox.Text = value; }
        }

        public bool IsReadOnly
        {
            get { return EditorBox.IsReadOnly; }
            set { EditorBox.IsReadOnly = value; }
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