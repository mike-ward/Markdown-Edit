using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Commands
{
    public class UndoEditCommandHander : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Execute, CanExecute));
            uiElement.InputBindings.Add(new KeyBinding(ApplicationCommands.Undo, Key.Z, ModifierKeys.Control));
            _textEditor = viewModel.TextEditor;
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _textEditor.CanUndo;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _textEditor.Undo();
        }
    }
}
