using System.Collections.Generic;
using System.Windows;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class EncodingComboBox
    {
        public static readonly DependencyProperty SelectedEncodingProperty = DependencyProperty.Register(
            "SelectedEncoding", typeof(MyEncodingInfo), typeof(EncodingComboBox),
            new PropertyMetadata(default(MyEncodingInfo)));

        public EncodingComboBox() { InitializeComponent(); }

        public MyEncodingInfo SelectedEncoding
        {
            get => (MyEncodingInfo)GetValue(SelectedEncodingProperty);
            set => SetValue(SelectedEncodingProperty, value);
        }

        public ICollection<MyEncodingInfo> SystemEncodings => MyEncodingInfo.GetEncodings();
    }
}