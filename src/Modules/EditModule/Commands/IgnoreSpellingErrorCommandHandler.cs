using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using EditModule.ViewModels;
using Infrastructure;

namespace EditModule.Commands
{
    public class IgnoreSpellingErrorCommandHandler : IEditCommandHandler
    {
        private readonly ISpellCheckService _spellCheckService;

        public IgnoreSpellingErrorCommandHandler(ISpellCheckService spellCheckService)
        {
            _spellCheckService = spellCheckService;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(EditingCommands.IgnoreSpellingError, Execute));
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            var word = (string)ea.Parameter;
            _spellCheckService.Add(word);
        }
    }
}
