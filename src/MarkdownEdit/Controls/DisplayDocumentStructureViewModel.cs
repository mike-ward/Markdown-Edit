using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CommonMark.Syntax;
using MarkdownEdit.Commands;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    internal class DisplayDocumentStructureViewModel : INotifyPropertyChanged
    {
        private DocumentStructure[] _structure;

        public DocumentStructure[] Structure
        {
            get => _structure;
            set => Set(ref _structure, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update(Block ast)
        {
            if (ast == null) return;

            Structure = AbstractSyntaxTree.EnumerateBlocks(ast.FirstChild)
                .Where(b => b.Tag == BlockTag.AtxHeading || b.Tag == BlockTag.SetextHeading)
                .Select(b => new DocumentStructure
                {
                    Heading = InlineContent(b.InlineContent),
                    Level = b.Heading.Level*20,
                    FontWeight = b.Heading.Level == 1 ? FontWeights.Bold : FontWeights.Normal,
                    Offset = b.SourcePosition
                })
                .ToArray();
        }

        private string InlineContent(Inline inline)
        {
            var content = inline.LiteralContent;
            if (inline.NextSibling != null) content += InlineContent(inline.NextSibling);
            return content;
        }

        public void Selected(int index)
        {
            var offset = index <= Structure.Length ? Structure[index].Offset : 0;
            ScrollToOffsetCommand.Command.Execute(offset, Application.Current.MainWindow);
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public struct DocumentStructure
        {
            public string Heading { get; set; }
            public int Level { get; set; }
            public FontWeight FontWeight { get; set; }
            public int Offset { get; set; }
        }
    }
}