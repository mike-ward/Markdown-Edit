using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class ToggleCodeCommandHandler : IEditCommandHandler
    {
        private readonly IEditService _editService;
        private TextEditor _textEditor;

        public ToggleCodeCommandHandler(IEditService editService)
        {
            _editService = editService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            var toggleCodeCommand = new RoutedCommand();
            uiElement.CommandBindings.Add(new CommandBinding(toggleCodeCommand, Execute));
            uiElement.InputBindings.Add(new KeyBinding(toggleCodeCommand, new KeyGesture(Key.K, ModifierKeys.Control)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _editService.AddRemoveText(_textEditor, "`");
        }
    }
}
