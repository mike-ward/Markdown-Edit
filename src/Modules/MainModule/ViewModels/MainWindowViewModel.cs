using Prism.Mvvm;

namespace MainModule.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "MARKDOWN EDIT";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
