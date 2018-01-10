using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace EditModule.Commands
{
    public class CorrectSpellingErrorCommandHandler : IEditCommandHandler
    {
        private TextEditor _textEditor;

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(EditingCommands.CorrectSpellingError, Execute));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            var parameters = (Tuple<string, TextSegment>)ea.Parameter;
            var word = parameters.Item1;
            var segment = parameters.Item2;
            _textEditor.Document.Replace(segment, word);
        }
    }
}
