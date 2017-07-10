using System.IO;
using System.Windows.Media;
using Infrastructure;
using Jot;
using Jot.Storage;
using Prism.Mvvm;

namespace ServicesModule
{
    public class Settings : BindableBase, ISettings
    {
        public static readonly string UserSettingsFolder = Path.Combine(Utility.AssemblyFolder(), "user-settings");
        public static StateTracker Tracker = new StateTracker { StoreFactory = new JsonFileStoreFactory(UserSettingsFolder) };

        private FontFamily _font = new FontFamily("Consolas");
        private double _fontSize = 16;
        private bool _wordWrap = true;

        public FontFamily Font
        {
            get => _font;
            set => SetProperty(ref _font, value);
        }

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize,  value);
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set => SetProperty(ref _wordWrap, value);
        }
    }
}
