using System.IO;
using Infrastructure;
using Prism.Events;
using Prism.Mvvm;

namespace MarkdownEdit
{
    public class ShellViewModel : BindableBase
    {
        private const string ProgramName = "MARKDOWN EDIT";

        public ShellViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<FileNameChangedEvent>().Subscribe(fileName => AppTitle = $"{ProgramName} - {Path.GetFileName(fileName)}");
        }

        private string _appTitle = ProgramName;
        public string AppTitle
        {
            get => _appTitle;
            set => SetProperty(ref _appTitle, value);
        }
    }
}
