using Infrastructure;
using Markdig;

namespace ServicesModule.Services
{
    public class MarkdigMarkdownEngine : IMarkdownEngine
    {
        public string Name { get; } = "GitHub Markdown";
        public string ToHtml(string text)
        {
            var pipelines = new MarkdownPipelineBuilder()
                .UseAbbreviations()
                .UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes()
                .UseGridTables()
                //.UseMathematics()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseDiagrams()
                .UseAutoLinks()
                .UseEmojiAndSmiley()
                .UseYamlFrontMatter();

            var pipeline = pipelines
                .UseGenericAttributes() // this one has to be last
                .Build();

            return Markdown.ToHtml(text, pipeline);
        }
    }
}
