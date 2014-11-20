using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public class FindReplaceSettings : INotifyPropertyChanged
    {
        private bool _caseSensitive;
        private bool _wholeWord;
        private bool _useRegex;
        private bool _useWildcards;

        public FindReplaceSettings()
        {
            CaseSensitive = Settings.Default.FindReplaceCaseSensitive;
            WholeWord = Settings.Default.FIndReplaceWholeWork;
            UseRegex = Settings.Default.FindReplaceRegex;
            UseWildcards = Settings.Default.FindReplaceWildcards;
        }

        public void Save()
        {
            Settings.Default.FindReplaceCaseSensitive = CaseSensitive;
            Settings.Default.FindReplaceRegex = UseRegex;
            Settings.Default.FIndReplaceWholeWork = WholeWord;
            Settings.Default.FindReplaceWildcards = UseWildcards;
        }

        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set { Set(ref _caseSensitive, value); }
        }

        public bool WholeWord
        {
            get { return _wholeWord; }
            set { Set(ref _wholeWord, value); }
        }

        public bool UseRegex
        {
            get { return _useRegex; }
            set { Set(ref _useRegex, value); }
        }

        public bool UseWildcards
        {
            get { return _useWildcards; }
            set { Set(ref _useWildcards, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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