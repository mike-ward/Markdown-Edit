using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LineContinuationTests
    {
        [TestMethod]
        public void OrderedListRegExTests()
        {
            MarkdownEdit.LineContinuationEnterCommand.OrderedListPattern.Match("3. Bla").Success.Should().BeTrue();
            MarkdownEdit.LineContinuationEnterCommand.OrderedListPattern.Match("3.Bla").Success.Should().BeFalse();
            MarkdownEdit.LineContinuationEnterCommand.OrderedListPattern.Match("3.   Bla").Success.Should().BeTrue();
            MarkdownEdit.LineContinuationEnterCommand.OrderedListPattern.Match("3.    Bla").Success.Should().BeFalse();
        }
    }
}
