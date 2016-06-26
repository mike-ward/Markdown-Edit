using FluentAssertions;
using MarkdownEdit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Models
{
    [TestClass]
    public class ListInputHandlerTests
    {
        [TestMethod]
        public void OrderedListRegExTests()
        {
            ListInputHandler.OrderedListPattern.Match("3. Bla").Success.Should().BeTrue();
            ListInputHandler.OrderedListPattern.Match("3.Bla").Success.Should().BeFalse();
            ListInputHandler.OrderedListPattern.Match("3.   Bla").Success.Should().BeTrue();
            ListInputHandler.OrderedListPattern.Match("3.    Bla").Success.Should().BeFalse();
            ListInputHandler.UnorderedListCheckboxPattern.Match("- [ ] this").Success.Should().BeTrue();
            ListInputHandler.UnorderedListCheckboxPattern.Match("- [x] is").Success.Should().BeTrue();
            ListInputHandler.UnorderedListCheckboxPattern.Match("- [X] a").Success.Should().BeTrue();
            ListInputHandler.UnorderedListCheckboxPattern.Match("- [Xx] test").Success.Should().BeFalse();
            ListInputHandler.UnorderedListCheckboxPattern.Match("-[X] !!!").Success.Should().BeFalse();
        }
    }
}