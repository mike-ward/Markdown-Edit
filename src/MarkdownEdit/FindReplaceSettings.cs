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
            set
            {
                if (_caseSensitive == value) return;
                _caseSensitive = value;
                OnPropertyChanged();
            }
        }

        public bool WholeWord
        {
            get { return _wholeWord; }
            set
            {
                if (_wholeWord == value) return;
                _wholeWord = value;
                OnPropertyChanged();
            }
        }

        public bool UseRegex
        {
            get { return _useRegex; }
            set
            {
                if (_useRegex == value) return;
                _useRegex = value;
                OnPropertyChanged();
            }
        }

        public bool UseWildcards
        {
            get { return _useWildcards; }
            set
            {
                if (_useWildcards == value) return;
                _useWildcards = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}