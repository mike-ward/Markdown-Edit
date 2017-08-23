using System.Windows;
using System.Windows.Input;
using EditModule.Dialogs;
using EditModule.ViewModels;
using Infrastructure;

namespace EditModule.Commands
{
    public class HelpCommandHandler : IEditCommandHandler
    {
        private HelpDialog _helpDialog;
        private readonly IMarkdownEngine[] _markdownEngines;

        public HelpCommandHandler(IMarkdownEngine[] markdownEngines)
        {
            _markdownEngines = markdownEngines;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, Execute));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            if (_helpDialog == null || !_helpDialog.IsLoaded)
            {
                _helpDialog = new HelpDialog(_markdownEngines[0])
                {
                    Owner = Application.Current.MainWindow
                };
            }

            if (_helpDialog.Visibility == Visibility.Visible) _helpDialog.Close();
            else _helpDialog.Show();
        }
    }
}