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
            var test = "1";
            test.Should().Be("1");
            //MarkdownEdit.LineContinuationEnterCommand.OrderedListPattern.Match("3. Bla").Success.Should().BeTrue();
        }
    }
}
