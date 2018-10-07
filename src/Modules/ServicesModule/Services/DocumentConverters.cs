using CliWrap;
using Infrastructure;

namespace ServicesModule.Services
{
    public class DocumentConverters : IDocumentConverters
    {
        private readonly INotify _notify;
        private readonly IPandoc _pandoc;

        private const string PandocCommonMarkFormatOptions =
            "markdown_strict"
            + "+fenced_code_blocks"
            + "+backtick_code_blocks"
            + "+intraword_underscores"
            + "+strikeout"
            + "+pipe_tables"
            + "+tex_math_dollars";

        public DocumentConverters(INotify notify, IPandoc pandoc)
        {
            _notify = notify;
            _pandoc = pandoc;
        }

        public string FromMicrosoftWord(string filename)
        {
            var result = _pandoc.Execute($"-f docx -t {PandocCommonMarkFormatOptions}", filename);

            if (result.ExitCode > 0)
            {
                _notify.Alert(result.Output);
                return null;
            }

            return result.Output;
        }
    }
}
