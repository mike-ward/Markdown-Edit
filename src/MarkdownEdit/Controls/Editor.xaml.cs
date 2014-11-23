using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
        private string _fileName;
        private string _displayName = string.Empty;
        private bool _wordWrap;
        private bool _spellCheck;
        private bool _autosave;
        private bool _isModified;
        private bool _removeSpecialCharacters;
        private EditorState _editorState = new EditorState();
        private const string F1ForHelp = " - F1 for Help";
        private readonly Action<string> _executeAutoSaveLater;

        public Editor()
        {
            InitializeComponent();
            EditBox.Loaded += EditBoxOnLoaded;
            EditBox.Unloaded += EditBoxOnUnloaded;
            EditBox.Options.IndentationSize = 2;
            EditBox.Options.EnableHyperlinks = false;
            EditBox.Options.ConvertTabsToSpaces = true;
            EditBox.Options.AllowScrollBelowDocument = true;
            EditBox.WordWrap = Settings.Default.WordWrapEnabled;
            EditBox.TextChanged += EditBoxOnTextChanged;
            EditBox.TextChanged += (s, e) => _executeAutoSaveLater(null);
            AutoSave = Settings.Default.AutoSave;
            PropertyChanged += OnSpellCheckChanged;
            DataObject.AddPastingHandler(EditBox, OnPaste);
            CommandBindings.Add(new CommandBinding(EditingCommands.CorrectSpellingError, ExecuteSpellCheckReplace));
            CommandBindings.Add(new CommandBinding(EditingCommands.IgnoreSpellingError, ExecuteAddToDictionary));
            _executeAutoSaveLater = Utility.Debounce<string>(s => Dispatcher.Invoke(ExecuteAutoSave), 4000);
        }

        private void EditBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // make scroll bar narrower
            var grid = EditBox.GetDescendantByType<Grid>();
            grid.ColumnDefinitions[1].Width = new GridLength(8);

            // remove indentation command in favor of ours
            var cmd = EditBox
                .TextArea
                .DefaultInputHandler
                .Editing
                .CommandBindings
                .FirstOrDefault(cb => cb.Command == AvalonEditCommands.IndentSelection);
            if (cmd != null) EditBox.TextArea.DefaultInputHandler.Editing.CommandBindings.Remove(cmd);

            // Speed up initial window display
            Task.Delay(10).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    InitializeSyntaxHighlighting();
                    ThemeChangedCallback(this, new DependencyPropertyChangedEventArgs());
                    EditBox.Focus();
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

        private void EditBoxOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Settings.Default.WordWrapEnabled = EditBox.WordWrap;
            Settings.Default.SpellCheckEnabled = SpellCheck;
            Settings.Default.AutoSave = AutoSave;
            FindReplaceDialog.Dispose();
        }

        private void EditorMenuOnContextMenuOpening(object sender, ContextMenuEventArgs ea)
        {
            var contextMenu = new ContextMenu();
            SpellCheckSuggestions(contextMenu);

            contextMenu.Items.Add(new MenuItem { Header = "Undo", Command = ApplicationCommands.Undo, InputGestureText = "Ctrl+Z" });
            contextMenu.Items.Add(new MenuItem { Header = "Redo", Command = ApplicationCommands.Redo, InputGestureText = "Ctrl+Y" });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut, InputGestureText = "Ctrl+X" });
            contextMenu.Items.Add(new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy, InputGestureText = "Ctrl+C" });
            contextMenu.Items.Add(new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste, InputGestureText = "Ctrl+V" });
            contextMenu.Items.Add(new MenuItem { Header = "Paste Special", Command = MainWindow.PasteSpecialCommand, InputGestureText = "Ctrl+Shift+V", ToolTip = "Paste smart quotes and hypens as plain text" });
            contextMenu.Items.Add(new MenuItem { Header = "Delete", Command = ApplicationCommands.Delete, InputGestureText = "Delete" });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem { Header = "Select All", Command = ApplicationCommands.SelectAll, InputGestureText = "Ctrl+A" });

            var element = (FrameworkElement)ea.Source;
            element.ContextMenu = contextMenu;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (_removeSpecialCharacters == false) return;
            _removeSpecialCharacters = false;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;
            var text = (string)e.SourceDataObject.GetData(DataFormats.UnicodeText);

            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, text.ReplaceSmartChars());
            e.DataObject = dataObject;
        }

        // Spell Check

        private void OnSpellCheckChanged(object o, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "SpellCheck") return;
            SpellCheckProvider.Enabled = SpellCheck;
            EditBox.Document.Insert(0, " ");
            EditBox.Document.UndoStack.Undo();
        }

        private void SpellCheckSuggestions(ContextMenu contextMenu)
        {
            if (SpellCheckProvider != null)
            {
                var editorPosition = EditBox.GetPositionFromPoint(Mouse.GetPosition(EditBox));
                if (!editorPosition.HasValue) return;
                var offset = EditBox.Document.GetOffset(editorPosition.Value.Line, editorPosition.Value.Column);
                var errorSegments = SpellCheckProvider.GetSpellCheckErrors();
                var misspelledSegment = errorSegments.FirstOrDefault(segment => segment.StartOffset <= offset && segment.EndOffset >= offset);
                if (misspelledSegment == null) return;

                // check if the clicked offset is the beginning or end of line to prevent snapping to it 
                // (like in text selection) with GetPositionFromPoint
                // in practice makes context menu not show when clicking on the first character of a line
                var currentLine = EditBox.Document.GetLineByOffset(offset);
                if (offset == currentLine.Offset || offset == currentLine.EndOffset) return;

                var misspelledText = EditBox.Document.GetText(misspelledSegment);
                var suggestions = SpellCheckProvider.GetSpellCheckSuggestions(misspelledText);
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
            SpellCheckProvider.Add(word);
            SpellCheck = !SpellCheck;
            SpellCheck = !SpellCheck;
        }

        // Commands

        private void Execute(Action action)
        {
            if (Execute(() => true)) action();
        }

        private bool Execute(Func<bool> action)
        {
            return EditBox.IsReadOnly ? Beep() : action();
        }

        private static bool Beep()
        {
            Utility.Beep();
            return false;
        }

        public void NewFile()
        {
            Execute(() =>
            {
                if (SaveIfModified() == false) return;
                Text = string.Empty;
                IsModified = false;
                FileName = string.Empty;
                Settings.Default.LastOpenFile = string.Empty;
            });
        }

        public void OpenFile(string file)
        {
            Execute(() =>
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
            });
        }

        public bool SaveIfModified()
        {
            return Execute(() =>
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
            });
        }

        public bool SaveFile()
        {
            return Execute(() => string.IsNullOrWhiteSpace(FileName)
                ? SaveFileAs()
                : Save());
        }

        public void ExecuteAutoSave()
        {
            if (AutoSave == false || IsModified == false || string.IsNullOrEmpty(FileName)) return;
            Execute(() =>
            {
                if (AutoSave == false || IsModified == false || string.IsNullOrEmpty(FileName)) return;
                SaveFile();
            });
        }

        public bool SaveFileAs()
        {
            return Execute(() =>
            {
                var dialog = new SaveFileDialog
                {
                    FilterIndex = 0,
                    OverwritePrompt = true,
                    RestoreDirectory = true,
                    Filter = @"Markdown files (*.md)|*.md|All files (*.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    FileName = dialog.FileNames[0];
                    return Save() && LoadFile(FileName);
                }
                return false;
            });
        }

        public bool LoadFile(string file)
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
                RecentFilesDialog.UpdateRecentFiles(file);
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
                ExecuteAutoSave();
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
            Execute(() => FindReplaceDialog.ShowFindDialog());
        }

        public void ReplaceDialog()
        {
            Execute(() => FindReplaceDialog.ShowReplaceDialog());
        }

        public void FindNext()
        {
            Execute(() => FindReplaceDialog.FindNext());
        }

        public void FindPrevious()
        {
            Execute(() => FindReplaceDialog.FindPrevious());
        }

        public void Bold()
        {
            Execute(() => AddRemoveText("**"));
        }

        public void Italic()
        {
            Execute(() => AddRemoveText("*"));
        }

        public void Code()
        {
            Execute(() => AddRemoveText("`"));
        }

        public void InsertHeader(int num)
        {
            Execute(() =>
            {
                var line = EditBox.Document.GetLineByOffset(EditBox.CaretOffset);
                if (line != null)
                {
                    var header = new string('#', num) + " ";
                    EditBox.Document.Insert(line.Offset, header);
                }
            });
        }

        public void InsertTimeStamp()
        {
            EditBox.SelectedText = DateTime.UtcNow.ToLocalTime().ToString("g");
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
            EditBox.FontSize = App.UserSettings.EditorFontSize;
        }

        public void PasteSpecial()
        {
            Execute(() =>
            {
                _removeSpecialCharacters = true;
                EditBox.Paste();
            });
        }

        public void OpenUserDictionary()
        {
            Utility.EditFile(SpellCheckProvider.CustomDictionaryFile());
        }

        public void ScrollToLine(int line)
        {
            var max = Math.Max(1, EditBox.Document.LineCount);
            line = Math.Min(max, Math.Max(line, 1));
            EditBox.ScrollToLine(line);
            var offset = EditBox.Document.GetOffset(line, 0);
            EditBox.CaretOffset = offset;
        }

        public void SelectPreviousHeader()
        {
            SelectHeader(false);
        }

        public void SelectNextHeader()
        {
            SelectHeader(true);
        }

        private void SelectHeader(bool next)
        {
            var start = EditBox.SelectionStart + (next ? EditBox.SelectionLength : 0);
            var options = RegexOptions.Multiline | (next ? RegexOptions.None : RegexOptions.RightToLeft);
            var regex = new Regex("^#{1,6}[^#].*", options);
            var match = regex.Match(EditBox.Text, start);
            if (!match.Success)
            {
                Utility.Beep();
                return;
            }
            EditBox.Select(match.Index, match.Length);
            var loc = EditBox.Document.GetLocation(match.Index);
            EditBox.ScrollTo(loc.Line, loc.Column);
        }

        public bool Find(Regex find)
        {
            return Execute(() =>
            {
                try
                {
                    var previous = find.Options.HasFlag(RegexOptions.RightToLeft);

                    var start = previous
                        ? EditBox.SelectionStart
                        : EditBox.SelectionStart + EditBox.SelectionLength;

                    var match = find.Match(EditBox.Text, start);
                    if (!match.Success) // start again from beginning or end
                    {
                        match = find.Match(EditBox.Text, previous ? EditBox.Text.Length : 0);
                    }

                    if (match.Success)
                    {
                        EditBox.Select(match.Index, match.Length);
                        var loc = EditBox.Document.GetLocation(match.Index);
                        EditBox.ScrollTo(loc.Line, loc.Column);
                    }

                    return match.Success;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    return false;
                }
            });
        }

        public bool Replace(Regex find, string replace)
        {
            return Execute(() =>
            {
                try
                {
                    var input = EditBox.Text.Substring(EditBox.SelectionStart, EditBox.SelectionLength);
                    var match = find.Match(input);
                    var replaced = false;
                    if (match.Success && match.Index == 0 && match.Length == input.Length)
                    {
                        var replaceWith = match.Result(replace);
                        EditBox.Document.Replace(EditBox.SelectionStart, EditBox.SelectionLength, replaceWith);
                        replaced = true;
                    }

                    if (!Find(find) && !replaced) Utility.Beep();
                    return replaced;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    return false;
                }
            });
        }

        public void ReplaceAll(Regex find, string replace)
        {
            Execute(() =>
            {
                try
                {
                    var offset = 0;
                    EditBox.BeginChange();
                    foreach (Match match in find.Matches(EditBox.Text))
                    {
                        var replaceWith = match.Result(replace);
                        EditBox.Document.Replace(offset + match.Index, match.Length, replaceWith);
                        offset += replaceWith.Length - match.Length;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    EditBox.EndChange();
                }
            });
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
            ScrollChanged?.Invoke(this, scrollChangedEventArgs);
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
            set { Set(ref _fileName, value); }
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
            set { Set(ref _displayName, value); }
        }

        public bool WordWrap
        {
            get { return _wordWrap; }
            set { Set(ref _wordWrap, value); }
        }

        public bool SpellCheck
        {
            get { return _spellCheck; }
            set { Set(ref _spellCheck, value); }
        }

        public bool AutoSave
        {
            get { return _autosave; }
            set { Set(ref _autosave, value); }

        }
        public bool IsModified
        {
            get { return _isModified; }
            set { Set(ref _isModified, value); }
        }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            "Theme", typeof(Theme), typeof(Editor), new PropertyMetadata(default(Theme), ThemeChangedCallback));

        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        private static void ThemeChangedCallback(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor)source;
            var theme = editor.Theme;
            if (theme == null) return;
            var highlightDefinition = editor.EditBox.SyntaxHighlighting;
            if (highlightDefinition == null) return;

            UpdateHilightingColor(highlightDefinition.GetNamedColor("Heading"), theme.HighlightHeading);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("Emphasis"), theme.HighlightEmphasis);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("StrongEmphasis"), theme.HighlightStrongEmphasis);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("InlineCode"), theme.HighlightInlineCode);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("BlockCode"), theme.HighlightBlockCode);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("BlockQuote"), theme.HighlightBlockQuote);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("Link"), theme.HighlightLink);
            UpdateHilightingColor(highlightDefinition.GetNamedColor("Image"), theme.HighlightImage);

            editor.EditBox.SyntaxHighlighting = null;
            editor.EditBox.SyntaxHighlighting = highlightDefinition;
        }

        private static void UpdateHilightingColor(HighlightingColor highlightingColor, Highlight highlight)
        {
            highlightingColor.Foreground = new HighlightBrush(highlight.Foreground);
            highlightingColor.Background = new HighlightBrush(highlight.Background);
            highlightingColor.FontStyle = ConvertFontStyle(highlight.FontStyle);
            highlightingColor.FontWeight = ConvertFontWeight(highlight.FontWeight);
        }

        private static FontStyle? ConvertFontStyle(string style)
        {
            try
            {
                return string.IsNullOrWhiteSpace(style)
                    ? null
                    : new FontStyleConverter().ConvertFromString(style) as FontStyle?;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static FontWeight? ConvertFontWeight(string weight)
        {
            try
            {
                return string.IsNullOrWhiteSpace(weight)
                    ? null
                    : new FontWeightConverter().ConvertFromString(weight) as FontWeight?;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            "VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(Editor), new PropertyMetadata(default(ScrollBarVisibility)));

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ShowEndOfLineProperty = DependencyProperty.Register(
            "ShowEndOfLine", typeof(bool), typeof(Editor), new PropertyMetadata(default(bool), ShowEndOfLineChanged));

        private static void ShowEndOfLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor)source;
            editor.EditBox.Options.ShowEndOfLine = editor.ShowEndOfLine;
        }

        public bool ShowEndOfLine
        {
            get { return (bool)GetValue(ShowEndOfLineProperty); }
            set { SetValue(ShowEndOfLineProperty, value); }
        }

        public static readonly DependencyProperty ShowSpacesProperty = DependencyProperty.Register(
            "ShowSpaces", typeof(bool), typeof(Editor), new PropertyMetadata(default(bool), ShowSpacesChanged));

        private static void ShowSpacesChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor)source;
            editor.EditBox.Options.ShowSpaces = editor.ShowSpaces;
        }

        public bool ShowSpaces
        {
            get { return (bool)GetValue(ShowSpacesProperty); }
            set { SetValue(ShowSpacesProperty, value); }
        }

        public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register(
            "ShowLineNumbers", typeof(bool), typeof(Editor), new PropertyMetadata(default(bool)));

        public bool ShowLineNumbers
        {
            get { return (bool)GetValue(ShowLineNumbersProperty); }
            set { SetValue(ShowLineNumbersProperty, value); }
        }

        public static readonly DependencyProperty ShowTabsProperty = DependencyProperty.Register(
            "ShowTabs", typeof(bool), typeof(Editor), new PropertyMetadata(default(bool), ShowTabsChanged));

        private static void ShowTabsChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor)source;
            editor.EditBox.Options.ShowTabs = editor.ShowTabs;
        }

        public bool ShowTabs
        {
            get { return (bool)GetValue(ShowTabsProperty); }
            set { SetValue(ShowTabsProperty, value); }
        }

        public static readonly DependencyProperty SpellCheckProviderProperty = DependencyProperty.Register(
            "SpellCheckProvider", typeof(ISpellCheckProvider), typeof(Editor), new PropertyMetadata(default(ISpellCheckProvider), SpellCheckChanged));

        private static void SpellCheckChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Editor)source;
            editor.SpellCheckProvider.Initialize(editor);
            editor.SpellCheck = Settings.Default.SpellCheckEnabled;
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get { return (ISpellCheckProvider)GetValue(SpellCheckProviderProperty); }
            set { SetValue(SpellCheckProviderProperty, value); }
        }

        public static readonly DependencyProperty FindReplaceDialogProperty = DependencyProperty.Register(
            "FindReplaceDialog", typeof (FindReplaceDialog), typeof (Editor), new PropertyMetadata(default(FindReplaceDialog)));

        public FindReplaceDialog FindReplaceDialog
        {
            get { return (FindReplaceDialog)GetValue(FindReplaceDialogProperty); }
            set { SetValue(FindReplaceDialogProperty, value); }
        }

        public static readonly DependencyProperty HighlightCurrentLineProperty = DependencyProperty.Register(
            "HighlightCurrentLine", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool)));

        public bool HighlightCurrentLine
        {
            get { return (bool)GetValue(HighlightCurrentLineProperty); }
            set { SetValue(HighlightCurrentLineProperty, value); }
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }

        // Drag and drop

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) e.Effects = DragDropEffects.None;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null) return;
                Dispatcher.InvokeAsync(() => OpenFile(files[0]));
            }
        }
    }
}