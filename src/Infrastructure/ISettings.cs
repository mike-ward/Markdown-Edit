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
        bool WordWrap { get; set; }

        // Spell Check
        bool SpellCheckIgnoreCodeBlocks { get; set; }
        bool SpellCheckIgnoreMarkupTags { get; set; }
        bool SpellCheckIgnoreAllCaps { get; set; }
        bool SpellCheckIgnoreWordsWithDigits { get; set; }
    }
}