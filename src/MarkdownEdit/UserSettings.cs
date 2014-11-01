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
        private string _themeName;
        private Theme _theme;
        private ThemeCollection _themes = new ThemeCollection();

        public string ThemeName
        {
            get { return _themeName; }
            set
            {
                if (_themeName == value) return;
                _themeName = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
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

        public ThemeCollection Themes
        {
            get { return _themes; }
            set
            {
                if (_themes == value) return;
                _themes = value;
                OnPropertyChanged();
            }
        }

        public void Update()
        {
            var userSettings = Load();
            Themes = userSettings.Themes;
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
                var settings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
                settings.PropertyChanged += (s, e) => { if (e.PropertyName == "ThemeName") settings.Theme = settings.Themes[settings.ThemeName]; };
                settings.Theme = settings.Themes[settings.ThemeName];
                return settings;
            }
            var defaultSettings = new UserSettings {Themes = new ThemeCollection {new Theme()}};
            defaultSettings.ThemeName = defaultSettings.Themes[0].Name;
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