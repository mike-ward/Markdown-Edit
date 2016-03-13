using System.Windows;
using CommonMark.Syntax;

namespace MarkdownEdit.Controls
{
    public partial class DisplayDocumentStructure
    {
        public DisplayDocumentStructure()
        {
            InitializeComponent();
            IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (IsVisible) ((DisplayDocumentStructureViewModel)DataContext).Update(AbstractSyntaxTree);
        }

        private async void OnSelected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            await Dispatcher.InvokeAsync(() => ((DisplayDocumentStructureViewModel)DataContext).Selected(Headings.SelectedIndex));
        }

        public static readonly DependencyProperty AbstractSyntaxTreeProperty = DependencyProperty.Register(
            "AbstractSyntaxTree", typeof(Block), typeof(DisplayDocumentStructure), new PropertyMetadata(default(Block)));

        public Block AbstractSyntaxTree
        {
            get { return (Block)GetValue(AbstractSyntaxTreeProperty); }
            set { SetValue(AbstractSyntaxTreeProperty, value); }
        }
    }
}