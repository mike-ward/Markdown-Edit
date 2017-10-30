using System.IO;
using System.Windows.Input;
using Jot;
using Jot.Storage;

namespace Infrastructure
{
    public static class Globals
    {
        public static readonly string UserSettingsFolder = Path.Combine(Utility.AssemblyFolder(), "user-settings");
        public static StateTracker Tracker = new StateTracker { StoreFactory = new JsonFileStoreFactory(UserSettingsFolder) };
    }
}
