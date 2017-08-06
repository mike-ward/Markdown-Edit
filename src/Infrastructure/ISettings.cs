using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Infrastructure
{
    public interface ISettings : INotifyPropertyChanged
    {
        FontFamily Font { get; set; }
        double FontSize { get; set; }
        bool WordWrap { get; set; }
    }
}