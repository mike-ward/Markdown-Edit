using System.ComponentModel;

namespace MarkdownEdit.SpellCheck
{
    public enum SpellingLanguages
    {
        [Description("English (Australia)")]
        Australian,

        [Description("English (Canada)")]
        Canadian,

        [Description("English (United Kingdom)")]
        UnitedKingdom,

        [Description("English (United States)")]
        UnitedStates,

        [Description("German (Germany)")]
        Germany,

        [Description("Spanish (Spain)")]
        Spain,

        [Description("Russian (Russia)")]
        Russian,
		
		[Description("Danish (Denmark)")]
		Denmark
    }
}