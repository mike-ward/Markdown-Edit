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
        }
    }
}