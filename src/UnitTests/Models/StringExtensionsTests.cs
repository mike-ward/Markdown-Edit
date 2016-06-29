using FluentAssertions;
using MarkdownEdit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming

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
            var result = ((string)null).SurroundWith("\"");
            result.Should().Be("\"\"");
        }

        [TestMethod]
        public void SurroundWithShouldReturnEmptyStringGivenNullAndCalledOnNull()
        {
            var result = ((string)null).SurroundWith(null);
            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void SurroundWithShouldReturnOriginalStringGivenNull()
        {
            var result = "Markdown".SurroundWith(null);
            result.Should().Be("Markdown");
        }

        [TestMethod]
        public void UnsurroundWithShouldReturnOriginalStringWithoutQuotesGivenQuotes()
        {
            var result = "\"Markdown\"".UnsurroundWith("\"");
            result.Should().Be("Markdown");
        }

        [TestMethod]
        public void UnsurroundWithShouldRemoveQuotesFromBeginningWhenCalledOnStringWithOpeningQuoteOnly()
        {
            var result = "\"Markdown".UnsurroundWith("\"");
            result.Should().Be("Markdown");
        }

        [TestMethod]
        public void UnsurroundWithShouldNotRemoveCharactersFromMiddleOfString()
        {
            var result = "'please don't remove my apostrophe'".UnsurroundWith("'");
            result.Should().Be("please don't remove my apostrophe");
        }

        [TestMethod]
        public void UnsurroundWithShouldReturnOriginalStringGivenNull()
        {
            var result = "Markdown".UnsurroundWith(null);
            result.Should().Be("Markdown");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2013WithHyphen()
        {
            var result = "Anti\u2013matter".ReplaceSmartChars();
            result.Should().Be("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2014WithHyphen()
        {
            var result = "Anti\u2014matter".ReplaceSmartChars();
            result.Should().Be("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2015WithHyphen()
        {
            var result = "Anti\u2015matter".ReplaceSmartChars();
            result.Should().Be("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2017WithUnderline()
        {
            var result = "Under\u2017lined".ReplaceSmartChars();
            result.Should().Be("Under_lined");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2018WithApostrophe()
        {
            var result = "Can\u2018t have smart characters".ReplaceSmartChars();
            result.Should().Be("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2019WithApostrophe()
        {
            var result = "Can\u2019t have smart characters".ReplaceSmartChars();
            result.Should().Be("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201bWithApostrophe()
        {
            var result = "Can\u201bt have smart characters".ReplaceSmartChars();
            result.Should().Be("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2032WithApostrophe()
        {
            var result = "Can\u2032t have smart characters".ReplaceSmartChars();
            result.Should().Be("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2026WithEllipses()
        {
            var result = "To be continued\u2026".ReplaceSmartChars();
            result.Should().Be("To be continued...");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201cWithQuotationMarks()
        {
            var result = "Walk up and say \u201chello\"".ReplaceSmartChars();
            result.Should().Be("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201dWithQuotationMarks()
        {
            var result = "Walk up and say \u201dhello\"".ReplaceSmartChars();
            result.Should().Be("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201eWithQuotationMarks()
        {
            var result = "Walk up and say \u201ehello\"".ReplaceSmartChars();
            result.Should().Be("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2033WithQuotationMarks()
        {
            var result = "Walk up and say \u2033hello\"".ReplaceSmartChars();
            result.Should().Be("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201aWithComma()
        {
            var result = "Pause\u201a for emphasis".ReplaceSmartChars();
            result.Should().Be("Pause, for emphasis");
        }
    }
}