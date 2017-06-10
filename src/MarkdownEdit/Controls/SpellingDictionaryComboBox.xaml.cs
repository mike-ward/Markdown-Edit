using System.Windows;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public partial class SpellingDictionaryComboBox
    {
        public static readonly DependencyProperty SpellCheckProviderProperty = DependencyProperty.Register(
            "SpellCheckProvider", typeof(ISpellCheckProvider), typeof(SpellingDictionaryComboBox),
            new PropertyMetadata(default(ISpellCheckProvider),
                (o, args) =>
                {
                    var scb = (SpellingDictionaryComboBox)o;
                    scb.Dispatcher.InvokeAsync(() => scb.Languages.ItemsSource = scb.SpellCheckProvider?.Languages());
                }));

        public SpellingDictionaryComboBox() { InitializeComponent(); }

        public ISpellCheckProvider SpellCheckProvider
        {
            get => (ISpellCheckProvider)GetValue(SpellCheckProviderProperty);
            set => SetValue(SpellCheckProviderProperty, value);
        }
    }
}