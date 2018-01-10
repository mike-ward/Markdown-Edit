using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit.Document;
using Infrastructure;
using MahApps.Metro.IconPacks;

namespace EditModule.Features
{
    public class EditorContextMenu : IEditFeature
    {
        private readonly ISpellCheckService _spellCheckService;
        private EditControlViewModel _viewModel;

        public EditorContextMenu(ISpellCheckService spellCheckService)
        {
            _spellCheckService = spellCheckService;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.TextEditor.ContextMenu = new ContextMenu();
            _viewModel.TextEditor.ContextMenuOpening += OnOpen;
        }

        private void OnOpen(object sender, ContextMenuEventArgs e)
        {
            var contextMenu = _viewModel.TextEditor.ContextMenu;
            contextMenu?.Items.Clear();
            AddSpellCheckSuggestions();
            AddEditMenuItems(contextMenu);
        }

        private void AddSpellCheckSuggestions()
        {
            var editor = _viewModel.TextEditor;
            var contextMenu = _viewModel.TextEditor.ContextMenu;
            if (contextMenu == null) return;

            var editorPosition = editor.GetPositionFromPoint(Mouse.GetPosition(editor));
            if (!editorPosition.HasValue) return;
            var offset = editor.Document.GetOffset(editorPosition.Value.Line, editorPosition.Value.Column);

            var errorSegments = _viewModel.SpellCheckRenderer.ErrorSegments;
            var misspelledSegment = errorSegments?.FirstOrDefault(segment => segment.StartOffset <= offset && segment.EndOffset >= offset);
            if (misspelledSegment == null) return;

            // check if the clicked offset is the beginning or end of line to prevent snapping to it
            // (like in text selection) with GetPositionFromPoint
            // in practice makes context menu not show when clicking on the first character of a line
            var currentLine = editor.Document.GetLineByOffset(offset);
            if (offset == currentLine.Offset || offset == currentLine.EndOffset) return;

            var misspelledText = editor.Document.GetText(misspelledSegment);
            var suggestions = _spellCheckService.Suggestions(misspelledText);

            foreach (var suggestion in suggestions)
            {
                contextMenu.Items.Add(SpellSuggestMenuItem(suggestion, misspelledSegment));
            }

            contextMenu.Items.Add(new MenuItem
            {
                // Header = TranslationProvider.Translate("editor-menu-add-to-dictionary"),
                Header = "Add to Dictiionary",
                Command = EditingCommands.IgnoreSpellingError,
                CommandParameter = misspelledText
            });

            contextMenu.Items.Add(new Separator());
        }

        private static void AddEditMenuItems(ItemsControl contextMenu)
        {
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-undo"),
                Header = "Undo", 
                Command = ApplicationCommands.Undo,
                InputGestureText = "Ctrl+Z",
                Icon = GetMaterialIcon(PackIconMaterialKind.Undo)
            });
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-redo"),
                Header = "Redo",
                Command = ApplicationCommands.Redo,
                InputGestureText = "Ctrl+Y",
                Icon = GetMaterialIcon(PackIconMaterialKind.Redo)
            });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-cut"),
                Header = "Cut",
                Command = ApplicationCommands.Cut,
                InputGestureText = "Ctrl+X",
                Icon = GetMaterialIcon(PackIconMaterialKind.ContentCut)
            });
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-copy"),
                Header = "Copy",
                Command = ApplicationCommands.Copy,
                InputGestureText = "Ctrl+C",
                Icon = GetMaterialIcon(PackIconMaterialKind.ContentCopy)
            });
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-paste"),
                Header = "Paste",
                Command = ApplicationCommands.Paste,
                InputGestureText = "Ctrl+V",
                Icon = GetMaterialIcon(PackIconMaterialKind.ContentPaste)
            });
            //contextMenu.Items.Add(new MenuItem
            //{
            //    Header = TranslationProvider.Translate("editor-menu-paste-special"),
            //    Command = PasteSpecialCommand.Command,
            //    InputGestureText = "Ctrl+Shift+V",
            //    ToolTip = "Paste smart quotes and hypens as plain text"
            //});
            //contextMenu.Items.Add(new MenuItem
            //{
            //    Header = TranslationProvider.Translate("editor-menu-paste-from-html"),
            //    Command = PasteFromHtmlCommand.Command,
            //    InputGestureText = "Alt+V"
            //});
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-delete"),
                Header = "Delete",
                Command = ApplicationCommands.Delete,
                InputGestureText = "Delete",
                Icon = GetMaterialIcon(PackIconMaterialKind.Delete)
            });
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(new MenuItem
            {
                //Header = TranslationProvider.Translate("editor-menu-select-all"),
                Header = "Select All",
                Command = ApplicationCommands.SelectAll,
                InputGestureText = "Ctrl+A",
                Icon = GetMaterialIcon(PackIconMaterialKind.SelectAll)
            });
            //contextMenu.Items.Add(new MenuItem
            //{
            //    //Header = TranslationProvider.Translate("editor-menu-wrap-format"),
            //    Command = FormatCommand.Command,
            //    InputGestureText = "Alt+F"
            //});
            //contextMenu.Items.Add(new MenuItem
            //{
            //    //Header = TranslationProvider.Translate("editor-menu-unwrap-format"),
            //    Command = UnformatCommand.Command,
            //    InputGestureText = "Alt+Shift+F"
            //});
        }

        private static PackIconMaterial GetMaterialIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial
            {
                Kind = kind,
                Margin = new Thickness(10, 0, 0, 0),
                Width = 12,
                Height = 12
            };
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
    }
}
