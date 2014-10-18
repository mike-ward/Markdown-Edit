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
using MessageBox = System.Windows.Forms.MessageBox;

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
            public bool StateSaved { get; private set; }
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
                editor.CanExecute = false;
                StateSaved = true;
            }

            public void Restore(Editor editor)
            {
                editor.Text = _text;
                editor.IsModified = _isModified;
                editor.WordWrap = _wordWrap;
                editor.IsReadOnly = false;
                editor.CanExecute = _canExecute;
                editor.DisplayName = string.Empty;
                StateSaved = false;
                editor.Dispatcher.InvokeAsync(() => editor.EditBox.Focus());
            }
        }

        public Editor()
        {
            InitializeComponent();
            CanExecute = true;
            EditBox.Loaded += EditBoxOnLoaded;
            EditBox.Unloaded += EditBoxOnUnloaded;
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
            EditBox.TextArea.DefaultInputHandler.Editing.CommandBindings.Clear();

            Dispatcher.InvokeAsync(() =>
            {
                EditBox.TextChanged += EditBoxOnTextChanged;
                LoadFile(Settings.Default.LastOpenFile);
                EditBox.Focus();
            });
        }

        private static void EditBoxOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            FindReplaceDialog.CloseDialog();
        }

        // Commands

        public void NewFile()
        {
            if (SaveIfModified() == false) return;
            Text = string.Empty;
            IsModified = false;
            FileName = string.Empty;
            Settings.Default.LastOpenFile = string.Empty;
        }

        public void OpenFile()
        {
            if (SaveIfModified() == false) return;
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadFile(dialog.FileNames[0]);
        }

        public bool SaveIfModified()
        {
            if (IsModified == false) return true;

            var result = MessageBox.Show(
                string.Format(@"Save ""{0}""?", FileName),
                @"File Modified",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            return (result == DialogResult.Yes)
                ? SaveFile()
                : result == DialogResult.No;
        }

        public bool SaveFile()
        {
            return string.IsNullOrWhiteSpace(FileName)
                ? SaveAsFile()
                : Save();
        }

        public bool SaveAsFile()
        {
            var dialog = new SaveFileDialog
            {
                FilterIndex = 2,
                OverwritePrompt = true,
                RestoreDirectory = true,
                Filter = @"Markdown files (*.md|*.md|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileName = dialog.FileNames[0];
                return Save() && LoadFile(FileName);
            }
            return false;
        }

        private bool LoadFile(string file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    NewFile();
                    return true;
                }
                EditBox.Text = File.ReadAllText(file);
                Settings.Default.LastOpenFile = file;
                IsModified = false;
                FileName = file;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Load File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool Save()
        {
            try
            {
                File.WriteAllText(FileName, Text);
                IsModified = false;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Save File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void ToggleHelp()
        {
            if (_editorState.StateSaved)
            {
                _editorState.Restore(this);
                return;
            }
            _editorState.Save(this);
            Text = Properties.Resources.Help;
            EditBox.IsModified = false;
            DisplayName = "Help";
        }

        public void FindDialog()
        {
            FindReplaceDialog.ShowFindDialog(EditBox);
        }

        public void ReplaceDialog()
        {
            FindReplaceDialog.ShowReplaceDialog(EditBox);
        }

        public void Bold()
        {
            AddRemoveText("**");
        }

        public void Italic()
        {
            AddRemoveText("*");
        }

        private void AddRemoveText(string quote)
        {
            var selected = EditBox.SelectedText;

            if (string.IsNullOrEmpty(selected))
            {
                EditBox.Document.Insert(EditBox.TextArea.Caret.Offset, quote);
            }
            else
            {
                EditBox.SelectedText = (selected.StartsWith(quote) && selected.EndsWith(quote))
                    ? selected.UnsurroundWith(quote)
                    : selected.SurroundWith(quote);
            }
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
                return (string.IsNullOrWhiteSpace(_displayName) == false)
                    ? _displayName
                    : string.IsNullOrWhiteSpace(FileName) ? "New Document (F1 for Help)" : Path.GetFileName(FileName);
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