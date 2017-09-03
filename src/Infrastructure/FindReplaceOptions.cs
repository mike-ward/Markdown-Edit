namespace Infrastructure
{
    public class FindReplaceOptions
    {
        public string FindText { get; set; }
        public string ReplaceText { get; set; }
        public bool CaseSensitive { get; set; }
        public bool WholeWord { get; set; }
        public bool Regex { get; set; }
        public bool Wildcards { get; set; }
        public bool SearchUp { get; set; }
        public bool RightToLeft { get; set; }
    }
}
