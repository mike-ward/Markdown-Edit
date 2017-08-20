using System;
using System.Windows.Controls;
using CommonMark.Syntax;
using EditModule.ViewModels;
using Infrastructure;
using Prism.Events;

namespace EditModule.Features
{
    public class SynchronizedScroll : IEditFeature
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAbstractSyntaxTree _abstractSyntaxTree;
        private EditControlViewModel _viewModel;

        public SynchronizedScroll(IEventAggregator eventAggregator, IAbstractSyntaxTree abstractSyntaxTree)
        {
            _eventAggregator = eventAggregator;
            _abstractSyntaxTree = abstractSyntaxTree;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            _viewModel = viewModel;
            viewModel.TextEditor.TextArea.TextView.ScrollOffsetChanged += (s, ea) => _viewModel.Dispatcher.Invoke(PublishVisibleBlockNumber);
        }

        private void PublishVisibleBlockNumber()
        {
            var abs = _abstractSyntaxTree.GenerateAbstractSyntaxTree(_viewModel.TextEditor.Text);
            var textView = _viewModel.TextEditor.TextArea.TextView;
            var scrollViewer = _viewModel.TextEditor.GetDescendantByType<ScrollViewer>();

            if (textView.ScrollOffset.Y >= scrollViewer.ScrollableHeight)
            {
                Publish(int.MaxValue, 0);
                return;
            }

            var line = textView.GetDocumentLineByVisualTop(textView.ScrollOffset.Y);
            var number = 1;
            var blockOffset = line.Offset;
            var skipListItem = true;

            foreach (var block in _abstractSyntaxTree.EnumerateBlocks(abs.FirstChild))
            {
                if (block.Tag == BlockTag.List) skipListItem = block.ListData.IsTight;
                blockOffset = block.SourcePosition;
                if (block.SourcePosition >= line.Offset) break;
                if (block.Tag == BlockTag.ListItem && skipListItem) continue;
                number += 1;
            }

            var startOfBlock = _viewModel.TextEditor.Document.GetLineByOffset(blockOffset);
            var extra = line.LineNumber - startOfBlock.LineNumber;
            Publish(number, extra);
        }

        private void Publish(int number, int extra)
        {
            _eventAggregator.GetEvent<TextScrollOffsetChanged>().Publish(new Tuple<int, int>(number, extra));
        }
    }
}
