using System.Windows.Input;

namespace MarkdownEdit.Controls
{
    public partial class Donate
    {
        public Donate()
        {
            InitializeComponent();
        }

        private void DonateOnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void CloseOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}