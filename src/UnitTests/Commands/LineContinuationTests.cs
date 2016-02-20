using FluentAssertions;
using MarkdownEdit.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Commands
{
    [TestClass]
    public class LineContinuationTests
    {
        [TestMethod]
        public void OrderedListRegExTests()
        {
            LineContinuationEnterCommand.OrderedListPattern.Match("3. Bla").Success.Should().BeTrue();
            LineContinuationEnterCommand.OrderedListPattern.Match("3.Bla").Success.Should().BeFalse();
            LineContinuationEnterCommand.OrderedListPattern.Match("3.   Bla").Success.Should().BeTrue();
            LineContinuationEnterCommand.OrderedListPattern.Match("3.    Bla").Success.Should().BeFalse();
            LineContinuationEnterCommand.UnorderedListCheckboxPattern.Match("- [ ] this").Success.Should().BeTrue();
            LineContinuationEnterCommand.UnorderedListCheckboxPattern.Match("- [x] is").Success.Should().BeTrue();
            LineContinuationEnterCommand.UnorderedListCheckboxPattern.Match("- [X] a").Success.Should().BeTrue();
            LineContinuationEnterCommand.UnorderedListCheckboxPattern.Match("- [Xx] test").Success.Should().BeFalse();
            LineContinuationEnterCommand.UnorderedListCheckboxPattern.Match("-[X] !!!").Success.Should().BeFalse();
        }
    }
}