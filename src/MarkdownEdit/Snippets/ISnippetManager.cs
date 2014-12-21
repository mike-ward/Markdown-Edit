using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownEdit
{
    public interface ISnippetManager
    {
        void Load();
        void FindSnippet();
    }
}
