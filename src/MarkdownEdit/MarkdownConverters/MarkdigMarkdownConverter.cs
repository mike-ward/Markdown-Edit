using Markdig;

namespace MarkdownEdit.MarkdownConverters
{
    public class MarkdigMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseEmojiAndSmiley()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .Build();

            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
