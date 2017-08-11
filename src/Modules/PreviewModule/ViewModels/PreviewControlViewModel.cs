using System;
using Infrastructure;
using Prism.Events;
using Prism.Mvvm;

namespace PreviewModule.ViewModels
{
    internal class PreviewControlViewModel : BindableBase
    {
        private readonly IMarkdownEngine _markdownEngine;
        public Action<string> UpdateBrowserDelegate { get; set; }

        public PreviewControlViewModel(IEventAggregator eventAggregator, IMarkdownEngine[] markdownEngines)
        {
            _markdownEngine = markdownEngines[0];
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(OnTextUpdated);
        }

        private void OnTextUpdated(string text)
        {
            var html = _markdownEngine.ToHtml(text);
            UpdateBrowserDelegate(html);
        }
    }
}
