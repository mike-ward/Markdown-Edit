namespace MarkdownEdit.MarkdownConverters
{
    public class CommonMarkConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown, bool resolveUrls)
        {
            return CommonMark.CommonMarkConverter.Convert(markdown);
        }
    }
}