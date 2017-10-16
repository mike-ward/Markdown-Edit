using System.Collections.Generic;
using System.Windows;
using UserModule.Models;

namespace UserModule.Views
{
    public partial class EncodingChooser
    {
        public static readonly DependencyProperty SelectedEncodingProperty = DependencyProperty.Register(
            "SelectedEncoding", typeof(EncodingChooser), typeof(EncodingChooser),
            new PropertyMetadata(default(EncodingChooser)));

        public EncodingChooser()
        {
            InitializeComponent();
        }

        public MyEncodingInfo SelectedEncoding
        {
            get => (MyEncodingInfo)GetValue(SelectedEncodingProperty);
            set => SetValue(SelectedEncodingProperty, value);
        }

        public ICollection<MyEncodingInfo> SystemEncodings => MyEncodingInfo.GetEncodings();
    }
}
