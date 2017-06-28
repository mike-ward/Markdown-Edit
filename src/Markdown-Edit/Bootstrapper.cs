using System.Windows;
using MarkdownEdit.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;

namespace MarkdownEdit
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
