using System.Windows.Input;
using MahApps.Metro.Controls;
using MarkdownEdit.Commands;

namespace MarkdownEdit
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(UpdatePreviewCommand.Command, (s, a) => Preview.UpdatePreview(a.Parameter as string)));
        }
    }
}