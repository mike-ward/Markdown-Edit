using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Commands
{
    public class RedoEditCommandHander : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, Execute, CanExecute));
            uiElement.InputBindings.Add(new KeyBinding(ApplicationCommands.Redo, Key.Y, ModifierKeys.Control));
            _textEditor = viewModel.TextEditor;
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _textEditor.CanRedo;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _textEditor.Redo();
        }
    }
}
