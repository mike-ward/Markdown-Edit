using System.IO;
using System.Runtime;
using System.Windows;
using Infrastructure;

namespace MarkdownEdit
{
    public partial class App
    {
        public App()
        {
            // Enable Multi-JIT startup
            var profileRoot = Globals.UserSettingsFolder;
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);
            ProfileOptimization.StartProfile("Startup.profile");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
