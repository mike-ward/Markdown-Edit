using Infrastructure;
using Prism.Commands;

namespace EditModule.Commands
{
    public class OpenDialogCommand : DelegateCommand
    {
        public OpenDialogCommand(OpenCommand openCommand, IFileActions fileActions)
            : base(() => Execute(openCommand, fileActions))
        {
        }

        public static void Execute(OpenCommand openCommand, IFileActions fileActions)
        {
            var file = fileActions.OpenDialog();
            if (file != null) openCommand.Execute(file);
        }
    }
}
