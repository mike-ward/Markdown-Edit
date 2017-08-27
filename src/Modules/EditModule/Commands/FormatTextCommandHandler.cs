using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class FormatTextCommandHandler : IEditCommandHandler
    {
        private readonly IFormatMarkdown _formatMarkdown;

        private TextEditor _textEditor;
        private static readonly RoutedCommand Command = new RoutedCommand();

        public FormatTextCommandHandler(IFormatMarkdown formatMarkdown)
        {
            _formatMarkdown = formatMarkdown;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(Command, Execute));
            uiElement.InputBindings.Add(new KeyBinding(Command, new KeyGesture(Key.F, ModifierKeys.Alt)));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            var result = _formatMarkdown.Format(_textEditor.Text);
            if (result != null) _textEditor.Text = result;
        }
    }
}

