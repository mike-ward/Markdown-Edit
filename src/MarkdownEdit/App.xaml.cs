using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MarkdownEdit.Properties;
using MarkdownEdit.SpellCheck;
using TinyIoC;

namespace MarkdownEdit
{
    public partial class App
    {
        private FileSystemWatcher _userSettingsWatcher;

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            InitializeSettings();

            var container = TinyIoCContainer.Current;
            container.Register<IMarkdownConverter, CommonMarkConverter>();
            container.Register<ISpellingService, SpellingService>();
            container.Register<ISpellCheckProvider, SpellCheckProvider>();

            var spellingService = container.Resolve<ISpellingService>();
            spellingService.SetLanguage(UserSettings.SpellCheckDictionary);
            UserSettings.PropertyChanged += (s, e) => { if (e.PropertyName == "SpellCheckDictonary") spellingService.SetLanguage(UserSettings.SpellCheckDictionary); };

            MainWindow = container.Resolve<MainWindow>();
            MainWindow.Show();
        }

        private void InitializeSettings()
        {
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }

            UserSettings = UserSettings.Load();

            Task.Factory.StartNew(() =>
            {
                _userSettingsWatcher = new FileSystemWatcher
                {
                    Path = UserSettings.SettingsFolder,
                    Filter = Path.GetFileName(UserSettings.SettingsFile),
                    NotifyFilter = NotifyFilters.LastWrite
                };
                _userSettingsWatcher.Changed += (s, e) => UserSettings.Update();
                _userSettingsWatcher.EnableRaisingEvents = true;
            });
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}