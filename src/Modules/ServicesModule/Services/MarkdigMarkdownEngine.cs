using Infrastructure;
using Markdig;

namespace ServicesModule.Services
{
    public class MarkdigMarkdownEngine : IMarkdownEngine
    {
        public string Name { get; } = "GitHub Markdown";
        public string ToHtml(string text)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .Build();

            return Markdown.ToHtml(text, pipeline);
        }
    }
}
