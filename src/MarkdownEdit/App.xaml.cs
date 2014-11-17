using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class App
    {
        private FileSystemWatcher _userSettingsWatcher;
        private CompositionContainer _container;

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            InitializeSettings();
            InitializeComponents();
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

            _userSettingsWatcher = new FileSystemWatcher
            {
                Path = UserSettings.SettingsFolder,
                Filter = Path.GetFileName(UserSettings.SettingsFile),
                NotifyFilter = NotifyFilters.LastWrite
            };
            _userSettingsWatcher.Changed += (s, e) => UserSettings.Update();
            _userSettingsWatcher.EnableRaisingEvents = true;
        }

        private void InitializeComponents()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}