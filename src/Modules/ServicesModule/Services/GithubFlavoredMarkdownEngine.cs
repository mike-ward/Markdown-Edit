using Infrastructure;

namespace ServicesModule.Services
{
    public class GithubFlavoredMarkdownEngine : IMarkdownEngine
    {
        private readonly IPandoc _pandoc;

        public GithubFlavoredMarkdownEngine(IPandoc pandoc)
        {
            _pandoc = pandoc;
        }

        public string Name { get; } = "GitHub Markdown";
        public string ToHtml(string text)
        {
            const string options = "-f gfm-emoji+tex_math_dollars -t html5 --email-obfuscation=none --mathjax --lua-filter=task-list.lua";
            var result = _pandoc.Execute(options, text);
            return result.Output.Replace(@"\$", "$");
        }
    }
}
