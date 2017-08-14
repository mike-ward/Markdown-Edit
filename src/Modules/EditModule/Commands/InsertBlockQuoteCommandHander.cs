using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class InsertBlockQuoteCommandHandler : IEditCommandHandler
    {
        private readonly IEditService _editService;
        private TextEditor _textEditor;

        public InsertBlockQuoteCommandHandler(IEditService editService)
        {
            _editService = editService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            var insertBlockQuoteCommand = new RoutedCommand();
            uiElement.CommandBindings.Add(new CommandBinding(insertBlockQuoteCommand, Execute));
            uiElement.InputBindings.Add(new KeyBinding(insertBlockQuoteCommand, new KeyGesture(Key.Q, ModifierKeys.Control)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            _editService.InsertBlockQuote(_textEditor);
        }
    }
}
