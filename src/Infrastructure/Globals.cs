using System.Diagnostics;
using System.IO;
using System.Reflection;
using Jot;
using Jot.Storage;

namespace Infrastructure
{
    public static class Globals
    {
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static readonly string ExecutingAssemblyName = ExecutingAssembly.GetName().CodeBase.Substring(8).Replace('/', '\\');

        public static readonly string AssemblyFolder = Path.GetDirectoryName(ExecutingAssemblyName);
        public static readonly string AssemblyVersion = FileVersionInfo.GetVersionInfo(ExecutingAssembly.Location).ProductVersion.Replace(".0", "");

        public static readonly string UserSettingsFolder = Path.Combine(AssemblyFolder, "user-settings");
        public static readonly StateTracker Tracker = new StateTracker { StoreFactory = new JsonFileStoreFactory(UserSettingsFolder) };
    }
}
