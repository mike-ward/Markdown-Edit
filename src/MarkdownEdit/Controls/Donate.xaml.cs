using System.Diagnostics;
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
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XGGZ8BEED7R62");
            Close();
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