using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using MarkdownEdit.Controls;

namespace MarkdownEdit.SpellCheck
{
    public interface ISpellCheckProvider
    {
        bool Enabled { get; set; }
        void Initialize(Editor editor);

        IEnumerable<TextSegment> GetSpellCheckErrors();

        IEnumerable<string> GetSpellCheckSuggestions(string word);

        void Add(string word);

        string CustomDictionaryFile();

        void Disconnect();

        string[] Languages();

        ISpellingService SpellingService();
    }
}