using System.Collections.Generic;

namespace MarkdownEdit.Models
{
    public interface IOutline
    {
        string OutlineType { get; }
        string OutlineLabel { get; }
        List<IOutline> Outlines { get; }
    }

    class DocumentOutline : IOutline
    {
        public string OutlineType { get; set; }
        public string OutlineLabel { get; set; }
        public List<IOutline> Outlines { get; set; }
    }
}