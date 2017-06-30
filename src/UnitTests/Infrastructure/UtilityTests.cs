using System;
using FluentAssertions;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Infrastructure
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void MemoizeShouldReturnFunction()
        {
            var mul = 2;
            // ReSharper disable once AccessToModifiedClosure
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
    }
}