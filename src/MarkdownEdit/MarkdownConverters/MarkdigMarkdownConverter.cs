using Markdig;

namespace MarkdownEdit.MarkdownConverters
{
    public class MarkdigMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
