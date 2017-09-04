using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using EditModule.Views;
using ICSharpCode.AvalonEdit;

namespace EditModule.Commands
{
    public class FindDialogCommandHandler : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, Execute));
            uiElement.InputBindings.Add(new KeyBinding(ApplicationCommands.Find, new KeyGesture(Key.F, ModifierKeys.Control)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            FindReplaceDialog.ShowFindReplace(_textEditor);
        }
    }
}
