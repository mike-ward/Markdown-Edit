using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MarkdownEdit.Properties;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public class UserSettings : INotifyPropertyChanged
    {
        private Theme _theme;
        private string _editorFontFamily = "Segoe UI";
        private double _editorFontSize = 14;

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

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme == value) return;
                _theme = value;
                OnPropertyChanged();
            }
        }

        public void Update()
        {
            var userSettings = Load();
            EditorFontFamily = userSettings.EditorFontFamily;
            EditorFontSize = userSettings.EditorFontSize;
            Theme = userSettings.Theme;
        }

        [JsonIgnore]
        public StringCollection RecentFiles
        {
            get { return Settings.Default.RecentFiles ?? (Settings.Default.RecentFiles = new StringCollection()); }
            set
            {
                if (Settings.Default.RecentFiles == value) return;
                Settings.Default.RecentFiles = value;
                OnPropertyChanged();
            }
        }

        public void UpdateRecentFiles(string file)
        {
            var recent = RecentFiles;
            recent.Remove(file);
            recent.Insert(0, file);
            var sc = new StringCollection(); // string collections suck
            sc.AddRange(recent.Cast<string>().Take(20).ToArray());
            RecentFiles = sc;
        }

        // Serialization

        public static string SettingsFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Markdown Edit"); }
        }

        public static string SettingsFile
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
            if (File.Exists(SettingsFile))
            {
                return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
            }
            var defaultSettings = new UserSettings {Theme = new Theme()};
            defaultSettings.Save();
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