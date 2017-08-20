using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;

namespace EditModule.Commands
{
    public class ConvertSelectionToListCommandHandler : IEditCommandHandler
    {
        private EditControlViewModel _viewModel;
        public static readonly RoutedCommand Command = new RoutedCommand();

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            _viewModel = viewModel;
            uiElement.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            var textEditor = _viewModel.TextEditor;
            var selection = textEditor.TextArea.Selection;
            var start = Math.Min(selection.StartPosition.Line, selection.EndPosition.Line);
            if (start == 0) return;
            var end = Math.Max(selection.StartPosition.Line, selection.EndPosition.Line);
            var document = textEditor.Document;
            var ordered = new Regex(@"^\s*\d+\.\s{1,4}");
            var unordered = new Regex(@"^\s*[-|\*|\+]\s{1,4}");

            textEditor.BeginChange();
            var index = 1;
            try
            {
                foreach (var num in Enumerable.Range(start, end - start + 1))
                {
                    var line = document.GetLineByNumber(num);
                    var offset = line.Offset;
                    var text = document.GetText(line);
                    if (string.IsNullOrWhiteSpace(text)) continue;

                    if (unordered.IsMatch(text))
                    {
                        var numbered = Regex.Replace(text, @"[-|\*|\+]", $"{index++}.");
                        document.Remove(line);
                        document.Insert(offset, numbered);
                    }
                    else if (ordered.IsMatch(text))
                    {
                        var unnumbered = Regex.Replace(text, @"\d+\.", "-");
                        document.Remove(line);
                        document.Insert(offset, unnumbered);
                    }
                    else
                    {
                        var indexOfFirstChar = text.TakeWhile(char.IsWhiteSpace).Count();
                        document.Insert(line.Offset + indexOfFirstChar, "- ");
                    }
                }
            }
            finally
            {
                textEditor.EndChange();
            }
        }
    }
}
