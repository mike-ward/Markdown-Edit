using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MarkdownEdit.Controls
{
    public partial class EncodingComboBox
    {
        public static readonly DependencyProperty SelectedEncodingProperty = DependencyProperty.Register(
            "SelectedEncodingFamily", typeof(EncodingInfo), typeof(FontComboBox),
            new PropertyMetadata(default(EncodingInfo)));

        public EncodingComboBox()
        {
            InitializeComponent();
        }

        public EncodingInfo SelectedEncoding
        {
            get { return (EncodingInfo) GetValue(SelectedEncodingProperty); }
            set { SetValue(SelectedEncodingProperty, value); }
        }

        public ICollection<EncodingInfo> SystemEncodings => Encoding.GetEncodings();
    }
}