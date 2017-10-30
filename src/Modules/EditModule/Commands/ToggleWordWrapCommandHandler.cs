using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Commands
{
    public class ToggleWordWrapCommandHandler : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            var command = new RoutedCommand();
            uiElement.CommandBindings.Add(new CommandBinding(command, Execute));
            _textEditor = viewModel.TextEditor;
            _textEditor.TextArea.InputBindings.Add(new KeyBinding(command, new KeyGesture(Key.W, ModifierKeys.Control)));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _textEditor.WordWrap = !_textEditor.WordWrap;
        }
    }
}
