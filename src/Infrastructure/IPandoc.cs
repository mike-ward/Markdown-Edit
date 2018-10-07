namespace Infrastructure
{
    public interface IPandoc
    {
        (int ExitCode, string Output) Execute(string options, string input);
    }
}