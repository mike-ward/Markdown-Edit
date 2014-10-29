using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MarkdownEdit.Properties;
using MarkdownEdit.SpellCheck;
using Microsoft.Win32;

namespace MarkdownEdit
{
    public partial class Editor : INotifyPropertyChanged
    {
        private bool _canExecute;
        private string _fileName;
        private string _displayName = string.Empty;
        private bool _wordWrap;
        private bool _spellCheck;
        private bool _isModified;
        private EditorState _editorState = new EditorState();
        private readonly FindReplaceDialog _findReplaceDialog;
        private ISpellCheckProvider _spellCheckProvider;
        private const string F1ForHelp = " - F1 for Help";

        private struct EditorState
        {
            public bool StateSaved { get; private set; }
            private string _text;
            private bool _isModified;
            private bool _wordWrap;
            private bool _spellCheck;
            private bool _canExecute;

            public void Save(Editor editor)
            {
                _text = editor.Text;
                _isModified = editor.IsModified;
                _wordWrap = editor.WordWrap;
                _canExecute = editor._canExecute;
                _spellCheck = editor.SpellCheck;
                editor.IsModified = false;
                editor.WordWrap = true;
                editor.IsReadOnly = true;
                editor._canExecute = false;
                editor.SpellCheck = false;
                StateSaved = true;
            }

            public void Restore(Editor editor)
            {
                if (StateSaved == false) return;
                editor.Text = _text;
                editor.IsModified = _isModified;
                editor.WordWrap = _wordWrap;
                editor.IsReadOnly = false;
                editor._canExecute = _canExecute;
                editor.SpellCheck = _spellCheck;
                editor.DisplayName = string.Empty;
                StateSaved = false;
                editor.Dispatcher.Invoke(() => editor.EditBox.Focus());
            }
        }

        public Editor()
        {
            InitializeComponent();
            _canExecute = true;
            EditBox.Loaded += EditBoxOnLoaded;
            EditBox.Unloaded += EditBoxOnUnloaded;
            CommandBindings.Add(new CommandBinding(EditingCommands.CorrectSpellingError, ExecuteSpellCheckReplace));
            CommandBindings.Add(new CommandBinding(EditingCommands.IgnoreSpellingError, ExecuteAddToDictionary));
            _findReplaceDialog = new FindReplaceDialog(EditBox);
            InitializeSpellCheck();
        }

        private void EditBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            InitializeSyntaxHighlighting();

            EditBox.Options.IndentationSize = 2;
            EditBox.Options.ConvertTabsToSpaces = true;
            EditBox.Options.AllowScrollBelowDocument = true;
            EditBox.Options.EnableHyperlinks = false;
            EditBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            var cmd = EditBox.TextArea.DefaultInputHandler.Editing.CommandBindings.First(cb => cb.Command == AvalonEditCommands.IndentSelection);
            EditBox.TextArea.DefaultInputHandler.Editing.CommandBindings.Remove(cmd);
            EditBox.TextChanged += EditBoxOnTextChanged;
            PropertyChanged += OnSpellCheckChanged;

            Task.Delay(100).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    var fileToOpen = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
                    LoadFile(fileToOpen ?? Settings.Default.LastOpenFile);
                    EditBox.Focus();
                    EditBox.WordWrap = Settings.Default.WordWrapEnabled;
                    _spellCheckProvider.Initialize(this);
                    SpellCheck = Settings.Default.SpellCheckEnabled;
                });
                t.Dispose();
            });
        }

        private void InitializeSyntaxHighlighting()
        {
            var reader = new XmlTextReader(new StringReader(Properties.Resources.markdown_xshd));
            var xshd = HighlightingLoader.LoadXshd(reader);
            var highlighter = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            EditBox.SyntaxHighlighting = highlighter;
            reader.Close();
        }

        private void OnSpellCheckChanged(object o, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "SpellCheck") return;
            _spellCheckProvider.Enabled = SpellCheck;
            EditBox.Document.Insert(0, " ");
            EditBox.Document.UndoStack.Undo();
        }

        private void EditBoxOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _findReplaceDialog.AllowClose = true;
            _spellCheckProvider.Disconnect();
            _findReplaceDialog.Close();
            Settings.Default.WordWrapEnabled = EditBox.WordWrap;
            Settings.Default.SpellCheckEnabled = SpellCheck;
        }

        private void EditorMenuOnContextMenuOpening(object sender, ContextMenuEventArgs ea)
        {
            var contextMenu = new ContextMenu();
            SpellCheckSuggestions(contextMenu);

            contextMenu.Items.Add(new MenuItem {Header = "Undo", Command = ApplicationCommands.Undo, InputGestureText = "Ctrl+Z"});
            contextMenu.Items.Add(new MenuItem {Header = "Redo", Command = ApplicationCommands.Redo, InputGestureText = "Ctrl+Y"});
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem {Header = "Cut", Command = ApplicationCommands.Cut, InputGestureText = "Ctrl+X"});
            contextMenu.Items.Add(new MenuItem {Header = "Copy", Command = ApplicationCommands.Copy, InputGestureText = "Ctrl+C"});
            contextMenu.Items.Add(new MenuItem {Header = "Paste", Command = ApplicationCommands.Paste, InputGestureText = "Ctrl+V"});
            contextMenu.Items.Add(new MenuItem {Header = "Delete", Command = ApplicationCommands.Delete, InputGestureText = "Delete"});
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem {Header = "Select All", Command = ApplicationCommands.SelectAll, InputGestureText = "Ctrl+A"});

            var element = (FrameworkElement)ea.Source;
            element.ContextMenu = contextMenu;
        }

        // Spell Check

        private void InitializeSpellCheck()
        {
            var spellingService = new SpellingService();
            spellingService.SetLanguage(SpellingLanguages.UnitedStates);
            _spellCheckProvider = new SpellCheckProvider(spellingService);
        }

        private void SpellCheckSuggestions(ContextMenu contextMenu)
        {
            if (_spellCheckProvider != null)
            {
                var editorPosition = EditBox.GetPositionFromPoint(Mouse.GetPosition(EditBox));
                if (!editorPosition.HasValue) return;
                var offset = EditBox.Document.GetOffset(editorPosition.Value.Line, editorPosition.Value.Column);
                var errorSegments = _spellCheckProvider.GetSpellCheckErrors();
                var misspelledSegment = errorSegments.FirstOrDefault(segment => segment.StartOffset <= offset && segment.EndOffset >= offset);
                if (misspelledSegment == null) return;

                // check if the clicked offset is the beginning or end of line to prevent snapping to it 
                // (like in text selection) with GetPositionFromPoint
                // in practice makes context menu not show when clicking on the first character of a line
                var currentLine = EditBox.Document.GetLineByOffset(offset);
                if (offset == currentLine.Offset || offset == currentLine.EndOffset) return;

                var misspelledText = EditBox.Document.GetText(misspelledSegment);
                var suggestions = _spellCheckProvider.GetSpellCheckSuggestions(misspelledText);
                foreach (var item in suggestions) contextMenu.Items.Add(SpellSuggestMenuItem(item, misspelledSegment));
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Add to Dictionary",
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandParameter = misspelledText
                });
                contextMenu.Items.Add(new Separator());
            }
        }

        private static MenuItem SpellSuggestMenuItem(string header, TextSegment segment)
        {
            return new MenuItem
            {
                Header = header,
                FontWeight = FontWeights.Bold,
                Command = EditingCommands.CorrectSpellingError,
                CommandParameter = new Tuple<string, TextSegment>(header, segment)
            };
        }

        private void ExecuteSpellCheckReplace(object sender, ExecutedRoutedEventArgs ea)
        {
            var parameters = (Tuple<string, TextSegment>)ea.Parameter;
            var word = parameters.Item1;
            var segment = parameters.Item2;
            EditBox.Document.Replace(segment, word);
        }

        private void ExecuteAddToDictionary(object sender, ExecutedRoutedEventArgs ea)
        {
            var word = (string)ea.Parameter;
            _spellCheckProvider.Add(word);
            SpellCheck = !SpellCheck;
            SpellCheck = !SpellCheck;
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

        public void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _canExecute;
        }

        public void OpenFile(string file)
        {
            if (SaveIfModified() == false) return;
            if (string.IsNullOrWhiteSpace(file))
            {
                var dialog = new OpenFileDialog();
                var result = dialog.ShowDialog();
                if (result == false) return;
                file = dialog.FileNames[0];
            }
            LoadFile(file);
        }

        public bool SaveIfModified()
        {
            if (IsModified == false) return true;

            var result = MessageBox.Show(
                @"Save your changes?",
                @"Hey!",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            return (result == MessageBoxResult.Yes)
                ? SaveFile()
                : result == MessageBoxResult.No;
        }

        public bool SaveFile()
        {
            return string.IsNullOrWhiteSpace(FileName)
                ? SaveFileAs()
                : Save();
        }

        public bool SaveFileAs()
        {
            var dialog = new SaveFileDialog
            {
                FilterIndex = 0,
                OverwritePrompt = true,
                RestoreDirectory = true,
                Filter = @"Markdown files (*.md|*.md|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
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
                EditBox.ScrollToHome();
                Settings.Default.LastOpenFile = file;
                ((MainWindow)Application.Current.MainWindow).UserSettings.UpdateRecentFiles(file);
                IsModified = false;
                FileName = file;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Load File", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(e.Message, @"Save File", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void CloseHelp()
        {
            _editorState.Restore(this);
        }

        public void FindDialog()
        {
            _findReplaceDialog.ShowFindDialog();
        }

        public void ReplaceDialog()
        {
            _findReplaceDialog.ShowReplaceDialog();
        }

        public void FindNext()
        {
            _findReplaceDialog.FindNext();
        }

        public void FindPrevious()
        {
            _findReplaceDialog.FindPrevious();
        }

        public void Bold()
        {
            AddRemoveText("**");
        }

        public void Italic()
        {
            AddRemoveText("*");
        }

        public void InsertHeader(int num)
        {
            var line = EditBox.Document.GetLineByOffset(EditBox.CaretOffset);
            if (line != null)
            {
                var header = new string('#', num) + " ";
                EditBox.Document.Insert(line.Offset, header);
            }
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

        public void IncreaseFontSize()
        {
            EditBox.FontSize = EditBox.FontSize + 1;
        }

        public void DecreaseFontSize()
        {
            EditBox.FontSize = EditBox.FontSize > 5 ? EditBox.FontSize - 1 : EditBox.FontSize;
        }

        public void RestoreFontSize()
        {
            EditBox.FontSize = ((MainWindow)Application.Current.MainWindow).UserSettings.EditorFontSize;
        }

        public void WrapToColumn()
        {
            EditBox.Text = EditBox.Text.WrapToColumn();
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
                    : string.IsNullOrWhiteSpace(FileName)
                        ? "New Document" + F1ForHelp
                        : Path.GetFileName(FileName);
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

        public bool SpellCheck
        {
            get { return _spellCheck; }
            set
            {
                if (_spellCheck == value) return;
                _spellCheck = value;
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