using System.Windows;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public partial class SpellingDictionaryComboBox
    {
        public SpellingDictionaryComboBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SpellCheckProviderProperty = DependencyProperty.Register(
            "SpellCheckProvider", typeof (ISpellCheckProvider), typeof (SpellingDictionaryComboBox), new PropertyMetadata(default(ISpellCheckProvider),
                (o, args) =>
                {
                    var scb = (SpellingDictionaryComboBox)o;
                    scb.Languages.ItemsSource = scb.SpellCheckProvider.Languages();
                }));

        public ISpellCheckProvider SpellCheckProvider
        {
            get { return (ISpellCheckProvider)GetValue(SpellCheckProviderProperty); }
            set { SetValue(SpellCheckProviderProperty, value); }
        }
    }
}