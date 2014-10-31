using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace MarkdownEdit.SpellCheck
{
    public interface ISpellCheckProvider
    {
        void Initialize(Editor editor);
        bool Enabled { get; set; }
        IEnumerable<TextSegment> GetSpellCheckErrors();
        IEnumerable<string> GetSpellCheckSuggestions(string word);
        void Add(string word);
        string CustomDictionaryFile();
        void Disconnect();
    }
}