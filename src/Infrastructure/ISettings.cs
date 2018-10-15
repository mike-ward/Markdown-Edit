using System.ComponentModel;
using System.Windows.Media;

namespace Infrastructure
{
    public interface ISettings : INotifyPropertyChanged
    {
        // Editor
        string CurrentFileName { get; set; }
        FontFamily Font { get; set; }
        double FontSize { get; set; }
        bool AutoSave { get; set; }
        bool ShowLineNumbers { get; set; }
        bool OpenLastFile { get; set; }
        bool FormatTextOnSave { get; set; }
        bool RememberLastPosition { get; set; }
        bool HighlightCurrentLine { get; set; }
        bool ShowVerticalScrollbar { get; set; }
        bool ShowSpaces { get; set; }
        bool ShowLineEndings { get; set; }
        bool ShowTabs { get; set; }
        bool SynchronizeScrollPositions { get; set; }
        bool WordWrap { get; set; }

        // Spell Check
        bool SpellCheckEnable { get; set; }
        bool SpellCheckIgnoreCodeBlocks { get; set; }
        bool SpellCheckIgnoreMarkupTags { get; set; }
        bool SpellCheckIgnoreAllCaps { get; set; }
        bool SpellCheckIgnoreWordsWithDigits { get; set; }
        string SpellCheckDictionary { get; set; }

        // Preview
        MarkdownEngines MarkdownEngines { get; set; }

        // Other
        bool Donated { get; set; }
    }
}