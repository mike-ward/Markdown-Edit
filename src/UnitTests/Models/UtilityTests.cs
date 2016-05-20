using System;
using FluentAssertions;
using MarkdownEdit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Models
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void MemoizeShouldReturnFunction()
        {
            var mul = 2;
            Func<int, int> func = i => i * mul;
            var result = func.Memoize();
            result.Should().NotBeNull();
            result(2).Should().Be(4);
            mul = 3;
            result(2).Should().Be(4);
            result(3).Should().Be(9);
        }

        [TestMethod]
        public void DebouceShouldReturnFunction()
        {
            Action<int> func = i => { };
            var result = func.Debounce();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void SeparateFrontMatterTest()
        {
            const string text =
@"---
layout: page
title:  Downloads
---

### (a.k.a. The Goods)
A Windows Desktop Markdown Editor[Read more...](/ markdownedit)";

            var tuple = Markdown.SeperateFrontMatter(text);
            tuple.Item1.Should().Match("*---*");
            tuple.Item2.Should().StartWith("###");
        }

        [TestMethod]
        public void SuggestTitleTest()
        {
            const string text =
@"---
layout: page
title:  ""Friday Links #357""
---

### (a.k.a. The Goods)
A Windows Desktop Markdown Editor[Read more...](/ markdownedit)";

            var match = DateTime.Now.ToString("yyyy-MM-dd-") + "friday-links-357";
            var title = Markdown.SuggestFilenameFromTitle(text);
            title.Should().Be(match);
        }
    }
}