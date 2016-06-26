using FluentAssertions;
using MarkdownEdit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Models
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void SurroundWithShouldReturnQuotedStringGivenQuotes()
        {
            var result = "Markdown".SurroundWith("\"");
            result.Should().Be("\"Markdown\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnOriginalStringGivenEmptyString()
        {
            var result = "Markdown".SurroundWith(string.Empty);
            result.Should().Be("Markdown");
        }

        [TestMethod]
        public void SurroundWithShouldReturnTwoQuotesGivenQuotesAndCalledOnEmptyString()
        {
            var result = string.Empty.SurroundWith("\"");
            result.Should().Be("\"\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnTwoQuotesGivenQuotesAndCalledOnNull()
        {
            string originalString = null;
            var result = originalString.SurroundWith("\"");
            result.Should().Be("\"\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnEmptyStringGivenNullAndCalledOnNull()
        {
            string originalString = null;
            var result = originalString.SurroundWith(null);
            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void SurroundWithShouldReturnOriginalStringGivenNull()
        {
            var result = "Markdown".SurroundWith(null);
            result.Should().Be("Markdown");
        }
    }
}