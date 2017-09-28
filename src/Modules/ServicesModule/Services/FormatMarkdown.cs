using CliWrap;
using CliWrap.Models;
using Infrastructure;

namespace ServicesModule.Services
{
    public class FormatMarkdown : IFormatMarkdown
    {
        private readonly INotify _notify;

        private const string CommonMarkFormatOptions = "markdown_strict"
                                                       + "+fenced_code_blocks"
                                                       + "+backtick_code_blocks"
                                                       + "+intraword_underscores"
                                                       + "+strikeout"
                                                       + "+pipe_tables";

        private const string EscapedLineBreaksOption = "-escaped_line_breaks";

        public FormatMarkdown(INotify notify)
        {
            _notify = notify;
        }

        public string Format(string text)
        {
            const string fromFormat = CommonMarkFormatOptions + EscapedLineBreaksOption;
            const string toFormat = CommonMarkFormatOptions;
            return Execute($"-f {fromFormat} -t {toFormat}", text);
        }

        public string Unformat(string text)
        {
            const string fromFormat = CommonMarkFormatOptions + EscapedLineBreaksOption;
            const string toFormat = CommonMarkFormatOptions + "--wrap=none --atx-headers";
            return Execute($"-f {fromFormat} -t {toFormat}", text);
        }

        private string Execute(string options, string text)
        {
            var cli = new Cli("Apps\\pandoc.exe");
            var input = new ExecutionInput(options, text);
            var result = cli.Execute(input);

            if (result.HasError)
            {
                _notify.Alert(string.IsNullOrWhiteSpace(result.StandardError) 
                    ? "empty error response" 
                    : result.StandardError);

                return null;
            }

            return result.StandardOutput.Replace(@"\$", "$");
        }
    }
}
