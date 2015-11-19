namespace MarkdownEdit.MarkdownConverters
{
    public class CommonMarkConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            return CommonMark.CommonMarkConverter.Convert(markdown);
        }
    }
}