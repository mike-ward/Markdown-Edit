using System.IO;
using System.Windows;
using Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;

namespace MarkdownEdit
{
    public class ShellViewModel : BindableBase
    {
        public IUnityContainer Container { get; }
        public IEventAggregator EventAggregator { get; }

        public ShellViewModel(IUnityContainer container, IEventAggregator eventAggregator)
        {
            Container = container;
            EventAggregator = eventAggregator;
            AppTitle = Constants.ProgramName;
            EventAggregator.GetEvent<FileNameChangedEvent>().Subscribe(fileName => DocumentName = Path.GetFileName(fileName));
            EventAggregator.GetEvent<DocumentModifiedChangedEvent>().Subscribe(flag => DocoumentModified = flag ? "*" : "");
        }

        private string _appTitle;

        public string AppTitle
        {
            get => _appTitle;
            set => SetProperty(ref _appTitle, value);
        }

        private string _documentName = string.Empty;

        public string DocumentName
        {
            get => _documentName;
            set => SetProperty(ref _documentName, value, UpdateAppTitle);
        }

        private string _docoumentModified = string.Empty;

        public string DocoumentModified
        {
            get => _docoumentModified;
            set => SetProperty(ref _docoumentModified, value, UpdateAppTitle);
        }

        public void UpdateAppTitle()
        {
            var strings = Container.Resolve<IStrings>();
            AppTitle = $"{Constants.ProgramName} - {DocoumentModified} {(string.IsNullOrEmpty(DocumentName) ? strings.NewDocumentName : DocumentName)}";
        }

        public bool AskToSaveIfModified()
        {
            if (string.IsNullOrEmpty(DocoumentModified)) return true;
            var notify = Container.Resolve<INotify>();
            var strings = Container.Resolve<IStrings>();
            var result = notify.ConfirmYesNoCancel(strings.SaveYourChanges);
            if (result == MessageBoxResult.Cancel) return false;
            if (result == MessageBoxResult.No) return true;
            EventAggregator.GetEvent<SaveCommandEvent>().Publish();
            return !string.IsNullOrEmpty(DocumentName);
        }
    }
}
