using EditModule.ViewModels;

namespace EditModule.Views
{
    public partial class ImageDropDialog
    {
        private ImageDropDialogViewModel ViewModel => (ImageDropDialogViewModel)DataContext;

        public ImageDropDialog()
        {
            InitializeComponent();
            ContextMenu = ViewModel.CreateContextMenu();
        }
    }
}
