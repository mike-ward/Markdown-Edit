using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Models
{
    public class FindReplaceSettings : INotifyPropertyChanged
    {
        private bool _caseSensitive;
        private bool _useRegex;
        private bool _useWildcards;
        private bool _wholeWord;

        public FindReplaceSettings()
        {
            CaseSensitive = Settings.Default.FindReplaceCaseSensitive;
            WholeWord = Settings.Default.FIndReplaceWholeWork;
            UseRegex = Settings.Default.FindReplaceRegex;
            UseWildcards = Settings.Default.FindReplaceWildcards;
        }

        public bool CaseSensitive { get => _caseSensitive;
            set => Set(ref _caseSensitive, value);
        }

        public bool WholeWord { get => _wholeWord;
            set => Set(ref _wholeWord, value);
        }

        public bool UseRegex { get => _useRegex;
            set => Set(ref _useRegex, value);
        }

        public bool UseWildcards { get => _useWildcards;
            set => Set(ref _useWildcards, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Save()
        {
            Settings.Default.FindReplaceCaseSensitive = CaseSensitive;
            Settings.Default.FindReplaceRegex = UseRegex;
            Settings.Default.FIndReplaceWholeWork = WholeWord;
            Settings.Default.FindReplaceWildcards = UseWildcards;
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}