﻿using System;
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

        [TestMethod]
        public void UnsurroundWithShouldReturnOriginalStringWithoutQuotesGivenQuotes()
        {
            var result = "\"Markdown\"".UnsurroundWith("\"");
            result.ShouldBeEquivalentTo("Markdown");
        }

        [TestMethod]
        public void UnsurroundWithShouldRemoveQuotesFromBeginningWhenCalledOnStringWithOpeningQuoteOnly()
        {
            var result = "\"Markdown".UnsurroundWith("\"");
            result.ShouldBeEquivalentTo("Markdown");
        }

        [TestMethod]
        public void UnsurroundWithShouldNotRemoveCharactersFromMiddleOfString()
        {
            var result = "'please don't remove my apostrophe'".UnsurroundWith("'");
            result.ShouldBeEquivalentTo("please don't remove my apostrophe");
        }

        [TestMethod]
        public void UnsurroundWithShouldReturnOriginalStringGivenNull()
        {
            var result = "Markdown".UnsurroundWith(null);
            result.ShouldBeEquivalentTo("Markdown");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2013WithHyphen()
        {
            var result = "Anti\u2013matter".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2014WithHyphen()
        {
            var result = "Anti\u2014matter".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2015WithHyphen()
        {
            var result = "Anti\u2015matter".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Anti-matter");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2017WithUnderline()
        {
            var result = "Under\u2017lined".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Under_lined");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2018WithApostrophe()
        {
            var result = "Can\u2018t have smart characters".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2019WithApostrophe()
        {
            var result = "Can\u2019t have smart characters".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201bWithApostrophe()
        {
            var result = "Can\u201bt have smart characters".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2032WithApostrophe()
        {
            var result = "Can\u2032t have smart characters".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Can't have smart characters");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2026WithEllipses()
        {
            var result = "To be continued\u2026".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("To be continued...");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201cWithQuotationMarks()
        {
            var result = "Walk up and say \u201chello\"".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201dWithQuotationMarks()
        {
            var result = "Walk up and say \u201dhello\"".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201eWithQuotationMarks()
        {
            var result = "Walk up and say \u201ehello\"".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU2033WithQuotationMarks()
        {
            var result = "Walk up and say \u2033hello\"".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Walk up and say \"hello\"");
        }

        [TestMethod]
        public void ReplacesmartcharsShouldReplaceU201aWithComma()
        {
            var result = "Pause\u201a for emphasis".ReplaceSmartChars();
            result.ShouldBeEquivalentTo("Pause, for emphasis");
        }
    }
}
