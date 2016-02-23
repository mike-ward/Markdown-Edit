using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommonMark.Syntax;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    internal class DisplayDocumentStructureViewModel : INotifyPropertyChanged
    {
        private string[] _structure;

        public string[] Structure
        {
            get { return _structure; }
            set { Set(ref _structure, value); }
        }

        public void Update(Block ast)
        {
            Structure = AbstractSyntaxTree.DocumentStructure(ast);
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