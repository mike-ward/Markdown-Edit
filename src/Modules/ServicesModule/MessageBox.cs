using System.Windows;
using Infrastructure;

namespace ServicesModule
{
    public class MessageBox : IMessageBox
    {
        public void Alert(string message)
        {
            System.Windows.MessageBox.Show(Application.Current.MainWindow, message);
        }
    }
}
