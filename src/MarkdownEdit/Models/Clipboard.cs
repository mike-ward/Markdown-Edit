using System.Windows;
using MarkdownEdit.Controls;
using MarkdownEdit.i18n;

namespace MarkdownEdit.Models
{
    internal static class Clipboard
    {
        public static void ExportHtmlToClipboard(string markdown, bool includeTemplate = false)
        {
            var text = Markdown.RemoveYamlFrontMatter(markdown);
            var html = Markdown.ToHtml(text);
            if (includeTemplate) html = UserTemplate.InsertContent(html);
            CopyHtmlToClipboard(html);
        }

        private static void CopyHtmlToClipboard(string html)
        {
            System.Windows.Clipboard.SetText(html);
            var popup = new FadingPopupControl();
            var message = TranslationProvider.Translate("message-html-clipboard") as string;
            popup.ShowDialogBox(Application.Current.MainWindow, message);
        }
    }
}