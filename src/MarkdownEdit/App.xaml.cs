using System.Windows;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class App
    {
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            FindReplaceDialog.SaveSettings();
            Settings.Default.Save();
        }
    }
}