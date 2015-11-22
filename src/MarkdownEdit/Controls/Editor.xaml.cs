using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using MarkdownEdit.Commands;
using MarkdownEdit.Models;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;
using static System.Windows.Input.ApplicationCommands;
using static MarkdownEdit.i18n.TranslationProvider;

namespace MarkdownEdit.Controls
{
    public partial class Editor : INotifyPropertyChanged
    {
        private bool _isModified;
        private bool _removeSpecialCharacters;
        private string _fileName;
        private string _displayName = string.Empty;
        private EditorState _editorState = new EditorState();
        private readonly Action<string> _executeAutoSaveLater;
        private readonly string _f1ForHelp = (string) Translate("editor-f1-for-help");

        public static RoutedCommand DeselectCommand = new RoutedCommand();
        public static RoutedCommand FormatCommand = new RoutedCommand();
        public static RoutedCommand FormatWithLinkReferencesCommand = new RoutedCommand();
        public static RoutedCommand UnformatCommand = new RoutedCommand();
        public static RoutedCommand PasteSpecialCommand = new RoutedCommand();
        public static RoutedCommand FindNextCommand = new RoutedCommand();
        public static RoutedCommand FindPreviousCommand = new RoutedCommand();
        public static RoutedCommand MoveLineUpCommand = new RoutedCommand();
        public static RoutedCommand MoveLineDownCommand = new RoutedCommand();
        public static RoutedCommand ConvertSelectionToListCommand = new RoutedCommand();
        public static RoutedCommand InsertBlockQuoteCommand = new RoutedCommand();
        public static RoutedCommand RevertCommand = new RoutedCommand();
        public static RoutedCommand InsertHyperlinkCommand = new RoutedCommand();
        public static RoutedCommand InsertHyperlinkDialogCommand = new RoutedCommand();

        public Editor()
        {
            InitializeComponent();
            DataContext = this;
            IsVisibleChanged += OnIsVisibleChanged;
            EditBox.Options.IndentationSize = 2;
            EditBox.Options.EnableHyperlinks = false;
            EditBox.Options.ConvertTabsToSpaces = true;
            EditBox.Options.AllowScrollBelowDocument = true;
            EditBox.Options.EnableHyperlinks = false;
            EditBox.Options.EnableEmailHyperlinks = false;
            EditBox.TextChanged += EditBoxOnTextChanged;
            EditBox.TextChanged += (s, e) => _executeAutoSaveLater(null);
            EditBox.PreviewKeyDown += (s, e) => EditorSpellCheck.AppsKeyDown = e.Key == Key.Apps && e.IsDown;
            _executeAutoSaveLater = Utility.Debounce<string>(s => Dispatcher.Invoke(ExecuteAutoSave), 4000);
            SetupSyntaxHighlighting();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            Dispatcher.InvokeAsync(() =>
            {
                DataObject.AddPastingHandler(EditBox, OnPaste);
                StyleScrollBar();
                SetupIndentationCommandBinding();
                SetupTabSnippetHandler();
                SetupLineContinuationEnterCommandHandler();
                ThemeChangedCallback(this, new DependencyPropertyChangedEventArgs());
                EditBox.Focus();

                // fixes context menu not showing on first click
                ContextMenu = new ContextMenu();
                ContextMenu.Items.Add(new MenuItem());
            });
        }

        private void StyleScrollBar()
        {
            // make scroll bar narrower
            var grid = EditBox.GetDescendantByType<Grid>();
            grid.ColumnDefinitions[1].Width = new GridLength(8);
            grid.RowDefinitions[1].Height = new GridLength(8);
        }

        private void SetupIndentationCommandBinding()
        {
            var cmd = EditBox
                .TextArea
                .DefaultInputHandler
                .Editing
                .CommandBindings
                .FirstOrDefault(cb => cb.Command == AvalonEditCommands.IndentSelection);
            if (cmd != null) EditBox.TextArea.DefaultInputHandler.Editing.CommandBindings.Remove(cmd);
        }

        private void SetupTabSnippetHandler()
        {
            var editingKeyBindings = EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
            var tabBinding = editingKeyBindings.Single(b => b.Key == Key.Tab && b.Modifiers == ModifierKeys.None);
            EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.Remove(tabBinding);
            var newTabBinding = new KeyBinding(new SnippetTabCommand(EditBox, tabBinding.Command, SnippetManager), tabBinding.Key, tabBinding.Modifiers);
            EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.Add(newTabBinding);
            SnippetManager.Initialize();
        }

        private void SetupLineContinuationEnterCommandHandler()
        {
            var editingKeyBindings = EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.OfType<KeyBinding>();
            var enterBinding = editingKeyBindings.Single(b => b.Key == Key.Enter && b.Modifiers == ModifierKeys.None);
            EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.Remove(enterBinding);
            var newEnterBinding = new KeyBinding(new LineContinuationEnterCommand(EditBox, enterBinding.Command), enterBinding.Key, enterBinding.Modifiers);
            EditBox.TextArea.DefaultInputHandler.Editing.InputBindings.Add(newEnterBinding);
        }

        private void SetupSyntaxHighlighting()
        {
            var colorizer = new MarkdownHighlightingColorizer();
            TextChanged += (s, e) => colorizer.OnTextChanged(Text);
            ThemeChanged += (s, e) => colorizer.OnThemeChanged(e.Theme);
            EditBox.TextArea.TextView.LineTransformers.Add(colorizer);
        }

        private void PasteSpecial() => IfNotReadOnly(() =>
        {
            try
            {
                _removeSpecialCharacters = true;
                EditBox.Paste();
            }
            finally
            {
                _removeSpecialCharacters = false;
            }
        });

        private void OnPaste(object sender, DataObjectPastingEventArgs pasteEventArgs)
        {
            var text = (string) pasteEventArgs.SourceDataObject.GetData(DataFormats.UnicodeText, true);
            if (string.IsNullOrWhiteSpace(text)) return;
            if (_removeSpecialCharacters) text = text.ReplaceSmartChars();
            else if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
            {
                text = Images.IsImageUrl(text.TrimEnd())
                    ? $"![{EditBox.SelectedText}]({text})\n"
                    : string.IsNullOrEmpty(EditBox.SelectedText) ? $"<{text}>" : $"[{EditBox.SelectedText}]({text})";
            }
            else return;

            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, text);
            pasteEventArgs.DataObject = dataObject;
        }

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

                if (Images.HasImageExtension(files[0]))
                {
                    var dialog = new ImageDropDialog
                    {
                        Owner = Application.Current.MainWindow,
                        TextEditor = EditBox,
                        DragEventArgs = e
                    };
                    dialog.ShowDialog();
                }
                else
                {
                    Dispatcher.InvokeAsync(() => OpenFile(files[0]));
                }
            }
        }


        private void EditorMenuOnContextMenuOpening(object sender, ContextMenuEventArgs ea)
        {
            var contextMenu = new ContextMenu();
            SpellCheckSuggestions(contextMenu);

            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-undo"), Command = Undo, InputGestureText = "Ctrl+Z"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-redo"), Command = Redo, InputGestureText = "Ctrl+Y"});
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-cut"), Command = Cut, InputGestureText = "Ctrl+X"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-copy"), Command = Copy, InputGestureText = "Ctrl+C"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-paste"), Command = Paste, InputGestureText = "Ctrl+V"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-paste-special"), Command = PasteSpecialCommand, InputGestureText = "Ctrl+Shift+V", ToolTip = "Paste smart quotes and hypens as plain text"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-delete"), Command = Delete, InputGestureText = "Delete"});
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-select-all"), Command = SelectAll, InputGestureText = "Ctrl+A"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-wrap-format"), Command = FormatCommand, InputGestureText = "Alt+F"});
            contextMenu.Items.Add(new MenuItem {Header = Translate("editor-menu-unwrap-format"), Command = UnformatCommand, InputGestureText = "Alt+Shift+F"});

            var element = (FrameworkElement) ea.Source;
            element.ContextMenu = contextMenu;
        }

        // Spell Check

        private void SpellCheckSuggestions(ContextMenu contextMenu) => EditorSpellCheck.SpellCheckSuggestions(this, contextMenu);

        private void ExecuteSpellCheckReplace(object sender, ExecutedRoutedEventArgs ea)
        {
            var parameters = (Tuple<string, TextSegment>) ea.Parameter;
            var word = parameters.Item1;
            var segment = parameters.Item2;
            EditBox.Document.Replace(segment, word);
        }

        private void ExecuteAddToDictionary(object sender, ExecutedRoutedEventArgs ea)
        {
            var word = (string) ea.Parameter;
            SpellCheckProvider.Add(word);
            SpellCheck = !SpellCheck;
            SpellCheck = !SpellCheck;
        }

        // Command handlers and helpers

        private void IfNotReadOnly(Action action) => IfNotReadOnly(() =>
        {
            action();
            return true;
        });

        private bool IfNotReadOnly(Func<bool> action) => EditBox.IsReadOnly ? EditorUtilities.ErrorBeep() : action();

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !EditBox.IsReadOnly;
        }

        private void FormatTextHandler(Func<string, string> converter, bool? forceAllText)
        {
            var isSelectedText = !string.IsNullOrEmpty(EditBox.SelectedText) && !forceAllText.GetValueOrDefault(false);
            var originalText = isSelectedText ? EditBox.SelectedText : EditBox.Document.Text;
            var formattedText = converter(originalText);
            if (string.CompareOrdinal(formattedText, originalText) != 0)
            {
                if (isSelectedText) EditBox.SelectedText = formattedText;
                else EditBox.Document.Text = formattedText;
            }
        }

        private void ExecuteFormatText(object sender, ExecutedRoutedEventArgs ea) => IfNotReadOnly(() => FormatTextHandler(Markdown.Wrap, ea.Parameter as bool?));

        private void ExecuteFormatTextWithLinkReferences(object sender, ExecutedRoutedEventArgs ea) => IfNotReadOnly(() => FormatTextHandler(Markdown.WrapWithLinkReferences, ea.Parameter as bool?));

        private void ExecuteUnformatText(object sender, ExecutedRoutedEventArgs ea) => IfNotReadOnly(() => FormatTextHandler(Markdown.Unwrap, false));

        public void NewFile() => IfNotReadOnly(() => EditorLoadSave.NewFile(this));

        public void OpenFile(string file) => IfNotReadOnly(() => EditorLoadSave.OpenFile(this, file));

        public void InsertFile(string file) => IfNotReadOnly(() => EditorLoadSave.InsertFile(this, file));

        public bool SaveIfModified() => IfNotReadOnly(() => EditorLoadSave.SaveIfModified(this));

        public bool SaveFile() => IfNotReadOnly(() => EditorLoadSave.SaveFile(this));

        public bool SaveFileAs() => IfNotReadOnly(() => EditorLoadSave.SaveFileAs(this));

        public bool LoadFile(string file, bool updateCursorPosition = true) => EditorLoadSave.LoadFile(this, file, updateCursorPosition);

        public void ExecuteAutoSave()
        {
            if (AutoSave == false || IsModified == false || string.IsNullOrEmpty(FileName)) return;
            IfNotReadOnly(() =>
            {
                if (AutoSave == false || IsModified == false || string.IsNullOrEmpty(FileName)) return;
                SaveFile();
            });
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
            Text = LoadHelp();
            EditBox.IsModified = false;
            DisplayName = "Help";
        }

        public void CloseHelp() => _editorState.Restore(this);

        private FindReplaceDialog _findReplaceDialog;

        private FindReplaceDialog FindReplaceDialog => _findReplaceDialog ?? (_findReplaceDialog = new FindReplaceDialog(new FindReplaceSettings()));

        private void ExecutePasteSpecial(object sender, ExecutedRoutedEventArgs e) => PasteSpecial();

        private void ExecuteFindDialog(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => FindReplaceDialog.ShowFindDialog());

        public void ReplaceDialog() => IfNotReadOnly(() => FindReplaceDialog.ShowReplaceDialog());

        private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => FindReplaceDialog.FindNext());

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => FindReplaceDialog.FindPrevious());

        public void Bold() => IfNotReadOnly(() => EditBox.AddRemoveText("**"));

        public void Italic() => IfNotReadOnly(() => EditBox.AddRemoveText("*"));

        public void Code() => IfNotReadOnly(() => EditBox.AddRemoveText("`"));

        public void ExecuteMoveLineUp(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => EditorUtilities.MoveSegmentUp(EditBox));

        public void ExecuteMoveLineDown(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => EditorUtilities.MoveSegmentDown(EditBox));

        public void ExecuteConvertSelectionToList(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => EditorUtilities.ConvertSelectionToList(EditBox));

        public void ExecuteInsertBlockQuote(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => EditorUtilities.InsertBlockQuote(EditBox));

        public void ExecuteInsertHyperlinkDialog(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => new InsertHyperlinkDialog {Owner = Application.Current.MainWindow}.ShowDialog());

        public void ExecuteInsertHyperlink(object sender, ExecutedRoutedEventArgs e) => IfNotReadOnly(() => EditorUtilities.InsertHyperlink(EditBox, e.Parameter as string));

        public void InsertHeader(int num) => IfNotReadOnly(() =>
        {
            var line = EditBox.Document.GetLineByOffset(EditBox.CaretOffset);
            if (line != null)
            {
                var header = new string('#', num) + " ";
                EditBox.Document.Insert(line.Offset, header);
            }
        });

        public void IncreaseFontSize() => EditBox.FontSize = EditBox.FontSize + 1;

        public void DecreaseFontSize() => EditBox.FontSize = EditBox.FontSize > 5 ? EditBox.FontSize - 1 : EditBox.FontSize;

        public void RestoreFontSize() => EditBox.FontSize = App.UserSettings.EditorFontSize;

        public void OpenUserDictionary() => Utility.EditFile(SpellCheckProvider.CustomDictionaryFile());

        public void SelectPreviousHeader() => EditBox.SelectHeader(false);

        public void SelectNextHeader() => EditBox.SelectHeader(true);

        public bool Find(Regex find) => IfNotReadOnly(() => EditBox.Find(find));

        public bool Replace(Regex find, string replace) => IfNotReadOnly(() => EditBox.Replace(find, replace));

        public void ReplaceAll(Regex find, string replace) => IfNotReadOnly(() => EditBox.ReplaceAll(find, replace));

        private void ExecuteDeselectCommand(object sender, ExecutedRoutedEventArgs e) => EditBox.SelectionLength = 0;

        private void ExecuteRevertCommand(object sender, ExecutedRoutedEventArgs e) => OpenFile(FileName);

        // Event handlers

        public EventHandler TextChanged;

        private void EditBoxOnTextChanged(object sender, EventArgs eventArgs) => TextChanged?.Invoke(this, eventArgs);

        public event ScrollChangedEventHandler ScrollChanged;

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs ea) => ScrollChanged?.Invoke(this, ea);

        public EventHandler<ThemeChangedEventArgs> ThemeChanged;

        private void OnThemeChanged(ThemeChangedEventArgs ea) => ThemeChanged?.Invoke(this, ea);

        public class ThemeChangedEventArgs : EventArgs
        {
            public Theme Theme { get; set; }
        }

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

        // Bindable Properties

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
                        ? $"{Translate("editor-new-document")} {_f1ForHelp}"
                        : Path.GetFileName(FileName);
            }
            set { Set(ref _displayName, value); }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set { Set(ref _isModified, value); }
        }

        public static readonly DependencyProperty AutoSaveProperty = DependencyProperty.Register(
            "AutoSave", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool)));

        public bool AutoSave
        {
            get { return (bool) GetValue(AutoSaveProperty); }
            set { SetValue(AutoSaveProperty, value); }
        }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(
            "Theme", typeof (Theme), typeof (Editor), new PropertyMetadata(default(Theme), ThemeChangedCallback));

        public Theme Theme
        {
            get { return (Theme) GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static void ThemeChangedCallback(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor) source;
            editor.OnThemeChanged(new ThemeChangedEventArgs {Theme = editor.Theme});
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            "VerticalScrollBarVisibility", typeof (ScrollBarVisibility), typeof (Editor), new PropertyMetadata(default(ScrollBarVisibility)));

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility) GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ShowEndOfLineProperty = DependencyProperty.Register(
            "ShowEndOfLine", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool), ShowEndOfLineChanged));

        private static void ShowEndOfLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor) source;
            editor.EditBox.Options.ShowEndOfLine = editor.ShowEndOfLine;
        }

        public bool ShowEndOfLine
        {
            get { return (bool) GetValue(ShowEndOfLineProperty); }
            set { SetValue(ShowEndOfLineProperty, value); }
        }

        public static readonly DependencyProperty ShowSpacesProperty = DependencyProperty.Register(
            "ShowSpaces", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool), ShowSpacesChanged));

        private static void ShowSpacesChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor) source;
            editor.EditBox.Options.ShowSpaces = editor.ShowSpaces;
        }

        public bool ShowSpaces
        {
            get { return (bool) GetValue(ShowSpacesProperty); }
            set { SetValue(ShowSpacesProperty, value); }
        }

        public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register(
            "ShowLineNumbers", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool)));

        public bool ShowLineNumbers
        {
            get { return (bool) GetValue(ShowLineNumbersProperty); }
            set { SetValue(ShowLineNumbersProperty, value); }
        }

        public static readonly DependencyProperty ShowTabsProperty = DependencyProperty.Register(
            "ShowTabs", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool), ShowTabsChanged));

        private static void ShowTabsChanged(DependencyObject source, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor) source;
            editor.EditBox.Options.ShowTabs = editor.ShowTabs;
        }

        public bool ShowTabs
        {
            get { return (bool) GetValue(ShowTabsProperty); }
            set { SetValue(ShowTabsProperty, value); }
        }

        public static readonly DependencyProperty SpellCheckProviderProperty = DependencyProperty.Register(
            "SpellCheckProvider", typeof (ISpellCheckProvider), typeof (Editor), new PropertyMetadata(default(ISpellCheckProvider), SpellCheckProviderPropertyChanged));

        private static void SpellCheckProviderPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Editor) source;
            editor.SpellCheckProvider.Initialize(editor);
            editor.SpellCheckProvider.Enabled = editor.SpellCheck;
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get { return (ISpellCheckProvider) GetValue(SpellCheckProviderProperty); }
            set { SetValue(SpellCheckProviderProperty, value); }
        }

        public static readonly DependencyProperty HighlightCurrentLineProperty = DependencyProperty.Register(
            "HighlightCurrentLine", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool), HighlightCurrentLineChanged));

        private static void HighlightCurrentLineChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var editor = (Editor) source;
            editor.EditBox.Options.HighlightCurrentLine = editor.HighlightCurrentLine;
        }

        public bool HighlightCurrentLine
        {
            get { return (bool) GetValue(HighlightCurrentLineProperty); }
            set { SetValue(HighlightCurrentLineProperty, value); }
        }

        public static readonly DependencyProperty SnippetManagerProperty = DependencyProperty.Register(
            "SnippetManager", typeof (ISnippetManager), typeof (Editor), new PropertyMetadata(default(ISnippetManager)));

        public ISnippetManager SnippetManager
        {
            get { return (ISnippetManager) GetValue(SnippetManagerProperty); }
            set { SetValue(SnippetManagerProperty, value); }
        }

        public static readonly DependencyProperty WordWrapProperty = DependencyProperty.Register(
            "WordWrap", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool)));

        public bool WordWrap
        {
            get { return (bool) GetValue(WordWrapProperty); }
            set { SetValue(WordWrapProperty, value); }
        }

        public static readonly DependencyProperty SpellCheckProperty = DependencyProperty.Register(
            "SpellCheck", typeof (bool), typeof (Editor), new PropertyMetadata(default(bool), SpellCheckPropertyChanged));

        private static void SpellCheckPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs ea)
        {
            var editor = (Editor) dependencyObject;
            if (editor.SpellCheckProvider == null) return;
            editor.SpellCheckProvider.Enabled = (bool) ea.NewValue;
            editor.EditBox.Document.Insert(0, " ");
            editor.EditBox.Document.UndoStack.Undo();
        }

        public bool SpellCheck
        {
            get { return (bool) GetValue(SpellCheckProperty); }
            set { SetValue(SpellCheckProperty, value); }
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}