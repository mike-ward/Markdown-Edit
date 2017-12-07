using CliWrap;
using CliWrap.Models;
using Infrastructure;

namespace ServicesModule.Services
{
    public class DocumentConverters : IDocumentConverters
    {
        private readonly INotify _notify;

        private const string PandocCommonMarkFormatOptions =
            "markdown_strict"
            + "+fenced_code_blocks"
            + "+backtick_code_blocks"
            + "+intraword_underscores"
            + "+strikeout"
            + "+pipe_tables"
            + "+tex_math_dollars";

        public DocumentConverters(INotify notify)
        {
            _notify = notify;
        }

        public string FromMicrosoftWord(string filename)
        {
            var cli = new Cli("Apps\\pandoc.exe");
            var input = new ExecutionInput($"-f docx -t {PandocCommonMarkFormatOptions}", filename);
            var result = cli.Execute(input);

            if (result.HasError)
            {
                _notify.Alert(string.IsNullOrWhiteSpace(result.StandardError)
                    ? "empty error response"
                    : result.StandardError);

                return null;
            }

            return result.StandardOutput;
        }
    }
}
