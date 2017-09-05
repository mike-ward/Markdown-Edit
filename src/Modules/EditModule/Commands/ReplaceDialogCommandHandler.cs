using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using EditModule.Views;
using ICSharpCode.AvalonEdit;

namespace EditModule.Commands
{
    public class ReplaceDialogCommandHandler : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Replace, Execute));
            uiElement.InputBindings.Add(new KeyBinding(ApplicationCommands.Replace, new KeyGesture(Key.H, ModifierKeys.Control)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            FindReplaceDialog.ShowFindReplace(_textEditor, true);
        }
    }
}
