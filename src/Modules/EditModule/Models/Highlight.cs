using Infrastructure;

namespace EditModule.Models
{
    public class Highlight : IHighlight
    {
        public string Name { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public bool Underline { get; set; }
    }
}
