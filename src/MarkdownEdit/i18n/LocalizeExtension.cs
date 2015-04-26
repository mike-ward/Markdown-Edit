using System;
using System.Windows.Markup;

namespace MarkdownEdit.i18n
{
    internal class LocalizeExtension : MarkupExtension
    {
        private readonly string _key;

        public LocalizeExtension(string key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return TranslationProvider.Translate(_key);
        }
    }
}