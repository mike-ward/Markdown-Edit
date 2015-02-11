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
            Func<int, int> func = i => i;
            var result = func.Memoize();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void DebouceShouldReturnFunction()
        {
            Action<int> func = i => { };
            var result = func.Debounce();
            result.Should().NotBeNull();
        }
    }
}