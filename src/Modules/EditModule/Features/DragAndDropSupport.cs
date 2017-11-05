using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using Infrastructure;

namespace EditModule.Features
{
    public class DragAndDropSupport : IEditFeature
    {
        private readonly IImageService _imageService;

        public DragAndDropSupport(IImageService imageService)
        {
            _imageService = imageService;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            viewModel.TextEditor.DragEnter += OnDragEnter;
            viewModel.TextEditor.Drop += OnDrop;
        }

        public void OnDragEnter(object sender, DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop) == false) dea.Effects = DragDropEffects.None;
        }

        public void OnDrop(object sender, DragEventArgs dea)
        {
            if (dea.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (!(dea.Data.GetData(DataFormats.FileDrop) is string[] files)) return;

                if (_imageService.HasImageExtension(files[0]))
                {
                    //var dialog = new ImageDropDialog
                    //{
                    //    Owner = Application.Current.MainWindow,
                    //    TextEditor = ViewModel.TextEditor,
                    //    DocumentFileName = FileName,
                    //    DragEventArgs = dea
                    //};
                    //dialog.ShowDialog();
                }

                else
                {
                    ApplicationCommands.Open.Execute(files[0], sender as IInputElement);
                }
            }
        }
    }
}
