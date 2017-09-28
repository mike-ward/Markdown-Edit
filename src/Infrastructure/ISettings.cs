using System.ComponentModel;
using System.Windows.Media;

namespace Infrastructure
{
    public interface ISettings : INotifyPropertyChanged
    {
        string CurrentFileName { get; set; }
        FontFamily Font { get; set; }
        double FontSize { get; set; }
        bool WordWrap { get; set; }
    }
}