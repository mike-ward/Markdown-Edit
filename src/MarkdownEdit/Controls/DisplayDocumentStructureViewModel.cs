using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using CommonMark.Syntax;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    internal class DisplayDocumentStructureViewModel : INotifyPropertyChanged
    {
        public struct DocumentStructure
        {
            public string Heading { get; set; }
            public int Level { get; set; }
            public FontWeight FontWeight { get; set; }
            public int Offset { get; set; }
        }

        private DocumentStructure[] _structure;

        public DocumentStructure[] Structure
        {
            get { return _structure; }
            set { Set(ref _structure, value); }
        }

        public void Update(Block ast)
        {
            Structure = AbstractSyntaxTree.EnumerateBlocks(ast.FirstChild)
                .Where(b => b.Tag == BlockTag.AtxHeading || b.Tag == BlockTag.SetextHeading)
                .Select(b => new DocumentStructure
                {
                    Heading = b.InlineContent.LiteralContent,
                    Level = b.Heading.Level * 20,
                    FontWeight = b.Heading.Level == 1 ? FontWeights.Bold : FontWeights.Normal,
                    Offset = b.SourcePosition
                })
                .ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}