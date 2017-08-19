using System;
using HtmlAgilityPack;
using Infrastructure;
using Prism.Events;
using Prism.Mvvm;

namespace PreviewModule.ViewModels
{
    internal class PreviewControlViewModel : BindableBase
    {
        private readonly IMarkdownEngine _markdownEngine;
        public Action<string> UpdateBrowserDelegate { get; set; }
        public Action<int, int> ScrollToOffsetDelegate { get; set; }

        public PreviewControlViewModel(IEventAggregator eventAggregator, IMarkdownEngine[] markdownEngines)
        {
            _markdownEngine = markdownEngines[0];
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(OnTextUpdated);
            eventAggregator.GetEvent<TextScrollOffsetChanged>().Subscribe(OnTextScrollOffsetChanged);
        }

        private void OnTextUpdated(string text)
        {
            var html = _markdownEngine.ToHtml(text);
            var scrubbedHtml = ScrubHtml(html);
            UpdateBrowserDelegate(scrubbedHtml);
        }

        private void OnTextScrollOffsetChanged(Tuple<int, int> offseTuple)
        {
            ScrollToOffsetDelegate(offseTuple.Item1, offseTuple.Item2);
        }

        private static string GetIdName(int number) => $"mde-{number}";

        private static string ScrubHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            void Each(HtmlNodeCollection nodes, Action<HtmlNode> action)
            {
                if (nodes == null) return;
                foreach (var node in nodes) action.Invoke(node);
            }

            var idx = 1;
            string GetName() => GetIdName(idx++);

            // Inject anchors at all block level elements for scroll synchronization
            var nc = doc.DocumentNode.SelectNodes("//p|//h1|//h2|//h3|//h4|//h5|//h6|//ul|//ol|//li|//hr|//pre|//blockquote");
            Each(nc, node => { if (node.Name != "blockquote" || node.ParentNode.Name != "li") node.Attributes.Add("id", GetName()); });

            // Remove potentially harmful elements
            nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
            Each(nc, node => node.ParentNode.RemoveChild(node, false));

            // Remove hrefs to java/j/vbscript URLs
            nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            Each(nc, node => node.SetAttributeValue("href", "#"));

            // Remove img with refs to java/j/vbscript URLs
            nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            Each(nc, node => node.SetAttributeValue("src", "#"));

            // Remove on<Event> handlers from all tags
            nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload]");
            Each(nc, node =>
            {
                node.Attributes.Remove("onFocus");
                node.Attributes.Remove("onBlur");
                node.Attributes.Remove("onClick");
                node.Attributes.Remove("onMouseOver");
                node.Attributes.Remove("onMouseOut");
                node.Attributes.Remove("onDoubleClick");
                node.Attributes.Remove("onLoad");
                node.Attributes.Remove("onUnload");
            });

            // remove any style attributes that contain the word expression (IE evaluates this as script)
            nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
            Each(nc, node => node.Attributes.Remove("style"));

            return doc.DocumentNode.WriteTo();
        }
    }
}
