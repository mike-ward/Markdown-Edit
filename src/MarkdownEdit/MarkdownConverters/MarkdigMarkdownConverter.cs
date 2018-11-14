using Markdig;

namespace MarkdownEdit.MarkdownConverters
{
    public class MarkdigMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
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
                .UseGenericAttributes()
                .Build();

            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
