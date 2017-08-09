using System.Windows.Media;

namespace Infrastructure
{
    public interface IColorService
    {
        Color FromHtml(string html);
        Brush CreateBrush(string colorSpec);
    }
}