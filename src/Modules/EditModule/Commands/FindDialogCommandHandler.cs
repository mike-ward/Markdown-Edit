using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using EditModule.Views;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class FindDialogCommandHandler : IEditCommandHandler
    {
        private readonly IStrings _strings;
        private TextEditor _textEditor;

        public FindDialogCommandHandler(IStrings strings)
        {
            _strings = strings;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, Execute));
            uiElement.InputBindings.Add(new KeyBinding(ApplicationCommands.Find, new KeyGesture(Key.F, ModifierKeys.Control)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            FindReplaceDialog.ShowFindReplace(_textEditor, _strings);
        }
    }
}
