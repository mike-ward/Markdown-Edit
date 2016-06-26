using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            result.ShouldBeEquivalentTo("\"Markdown\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnOriginalStringGivenEmptyString()
        {
            var result = "Markdown".SurroundWith(string.Empty);
            result.ShouldBeEquivalentTo("Markdown");
        }

        [TestMethod]
        public void SurroundWithShouldReturnTwoQuotesGivenQuotesAndCalledOnEmptyString()
        {
            var result = String.Empty.SurroundWith("\"");
            result.ShouldBeEquivalentTo("\"\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnTwoQuotesGivenQuotesAndCalledOnNull()
        {
            string originalString = null;
            var result = originalString.SurroundWith("\"");
            result.ShouldBeEquivalentTo("\"\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnEmptyStringGivenNullAndCalledOnNull()
        {
            string originalString = null;
            var result = originalString.SurroundWith(null);
            result.ShouldBeEquivalentTo(string.Empty);
        }

        [TestMethod]
        public void SurroundWithShouldReturnOriginalStringGivenNull()
        {
            var result = "Markdown".SurroundWith(null);
            result.ShouldBeEquivalentTo("Markdown");
        }
    }
}
