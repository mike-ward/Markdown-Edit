using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
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

    public class UserSettings : INotifyPropertyChanged
    {
        private string _editorBackground = "#F7F4EF";
        private string _editorForeground = "#000000";
        private string _editorFontFamily = "Segoe UI";
        private double _editorFontSize = 14;

        [TypeConverter(typeof(BrushConverter))]
        public string EditorBackground
        {
            get { return _editorBackground; }
            set
            {
                if (_editorBackground == value) return;
                _editorBackground = value;
                OnPropertyChanged();
            }
        }

        [DefaultValue("#000000")]
        public string EditorForeground
        {
            get { return _editorForeground; }
            set
            {
                if (_editorForeground == value) return;
                _editorForeground = value;
                OnPropertyChanged();
            }
        }

        [DefaultValue("Segoe WP Semilight")]
        public string EditorFontFamily
        {
            get { return _editorFontFamily; }
            set
            {
                if (_editorFontFamily == value) return;
                _editorFontFamily = value;
                OnPropertyChanged();
            }
        }

        [DefaultValue(14)]
        public double EditorFontSize
        {
            get { return _editorFontSize; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_editorFontSize == value) return;
                _editorFontSize = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}