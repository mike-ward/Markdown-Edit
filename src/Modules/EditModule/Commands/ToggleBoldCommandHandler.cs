using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class ToggleBoldCommandHandler : IEditCommandHandler
    {
        private readonly IEditService _editService;
        private TextEditor _textEditor;

        public ToggleBoldCommandHandler(IEditService editService)
        {
            _editService = editService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBold, Execute));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _editService.AddRemoveText(_textEditor, "**");
        }
    }
}
