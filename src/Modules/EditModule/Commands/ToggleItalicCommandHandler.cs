using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class ToggleItalicCommandHandler : IEditCommandHandler
    {
        private readonly IEditService _editService;
        private TextEditor _textEditor;

        public ToggleItalicCommandHandler(IEditService editService)
        {
            _editService = editService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleItalic, Execute));
            _textEditor = viewModel.TextEditor;
            _textEditor.TextArea.InputBindings.Add(new KeyBinding(EditingCommands.ToggleItalic, new KeyGesture(Key.I, ModifierKeys.Control)));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _editService.AddRemoveText(_textEditor, "*");
        }
    }
}
