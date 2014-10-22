using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using MarkdownEdit.Properties;
using Newtonsoft.Json;

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

        // Serialization

        private static string SettingsFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Markdown Edit"); }
        }

        private static string SettingsFile
        {
            get { return Path.Combine(SettingsFolder, "user_settings.json"); }
        }

        public void Save()
        {
            Directory.CreateDirectory(SettingsFolder);
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static UserSettings Load()
        {
            if (File.Exists(SettingsFile)) return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
            new UserSettings().Save();
            return Load();
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}