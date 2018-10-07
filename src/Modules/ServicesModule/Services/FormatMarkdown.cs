using CliWrap;
using Infrastructure;

namespace ServicesModule.Services
{
    public class FormatMarkdown : IFormatMarkdown
    {
        private readonly INotify _notify;
        private readonly IPandoc _pandoc;

        private const string CommonMarkFormatOptions = "markdown_strict"
                                                       + "+fenced_code_blocks"
                                                       + "+backtick_code_blocks"
                                                       + "+intraword_underscores"
                                                       + "+strikeout"
                                                       + "+pipe_tables";

        private const string EscapedLineBreaksOption = "-escaped_line_breaks";

        public FormatMarkdown(INotify notify, IPandoc pandoc)
        {
            _notify = notify;
            _pandoc = pandoc;
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
            var result = _pandoc.Execute(options, text);

            if (result.ExitCode > 0)
            {
                _notify.Alert(result.Output);
                return null;
            }

            return result.Output.Replace(@"\$", "$");
        }
    }
}
