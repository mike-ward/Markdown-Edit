using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenDialogCommand : DelegateCommand
    {
        public OpenDialogCommand(OpenCommand openCommand, IOpenSaveActions openSaveActions)
            : base(() => Execute(openCommand, openSaveActions))
        {
        }

        public static void Execute(OpenCommand openCommand, IOpenSaveActions openSaveActions)
        {
            var file = openSaveActions.OpenDialog();
            if (file != null) openCommand.Execute(file);
        }
    }
}
