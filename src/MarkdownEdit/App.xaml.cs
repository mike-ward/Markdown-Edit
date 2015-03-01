using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MarkdownEdit.Controls;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;
using TinyIoC;

namespace MarkdownEdit
{
    public partial class App
    {
        public const string Title = "MARKDOWN EDIT";
        private FileSystemWatcher _userSettingsWatcher;

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            InitializeSettings();

            var container = TinyIoCContainer.Current;
            container.Register<IMarkdownConverter, CommonMarkConverter>();
            container.Register<ISpellingService, SpellingService>();
            container.Register<ISpellCheckProvider, SpellCheckProvider>();
            container.Register<ISnippetManager, SnippetManager>();

            var mainWindow = container.Resolve<MainWindow>();

            var windowPlacementSettings = mainWindow.GetWindowPlacementSettings();

            if (windowPlacementSettings.UpgradeSettings)
            {
                windowPlacementSettings.Upgrade();
                windowPlacementSettings.UpgradeSettings = false;
                windowPlacementSettings.Save();
            }

            MainWindow = mainWindow;
            MainWindow.Show();

            Task.Factory.StartNew(() =>
            {
                var spellingService = container.Resolve<ISpellingService>();
                spellingService.Language = UserSettings.SpellCheckDictionary;
                UserSettings.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(UserSettings.SpellCheckDictionary))
                        spellingService.Language = UserSettings.SpellCheckDictionary;
                };
                _userSettingsWatcher = Utility.WatchFile(UserSettings.SettingsFile, UserSettings.Update);
            });
        }

        private void InitializeSettings()
        {
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();

                // Adds new settings from this version
                UserSettings.Load()?.Save();
            }

            UserSettings = UserSettings.Load();
            if (UserSettings == null) Shutdown();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            _userSettingsWatcher?.Dispose();
        }
    }
}