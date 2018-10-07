using CliWrap;
using Infrastructure;

namespace ServicesModule.Services
{
    public class Pandoc : IPandoc
    {
        public (int ExitCode, string Output) Execute(string options, string input)
        {
            var result = new Cli("Apps\\pandoc.exe")
                .SetArguments(options)
                .SetStandardInput(input)
                .Execute();

            var output = result.ExitCode == 0 
                ? result.StandardOutput 
                : string.IsNullOrWhiteSpace(result.StandardError) ? "Empty error response" : result.StandardError;

            return (result.ExitCode, output);
        }
    }
}
