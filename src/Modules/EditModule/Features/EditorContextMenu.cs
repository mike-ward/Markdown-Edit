using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EditModule.ViewModels;
using MahApps.Metro.IconPacks;

namespace EditModule.Features
{
    public class EditorContextMenu : IEditFeature
    {

        public void Initialize(EditControlViewModel viewModel)
        {
            var control = viewModel.TextEditor;
            control.ContextMenu = new ContextMenu();
            AddEditMenuItems(control.ContextMenu);
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
    }
}
