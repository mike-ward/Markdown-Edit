using Infrastructure;

namespace ServicesModule
{
    public class CommonMarkEngine : IMarkdownEngine
    {
        public string ToHtml(string text)
        {
            return CommonMark.CommonMarkConverter.Convert(text);
        }
    }
}