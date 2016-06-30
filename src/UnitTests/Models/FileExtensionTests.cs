using System;
using FluentAssertions;
using MarkdownEdit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Models
{
    [TestClass]
    public class FileExtensionTests
    {
        [TestMethod]
        public void MakeRelativePathShouldReturnRelativePath()
        {
            var path = FileExtensions.MakeRelativePath(@"file://mystuff/test.txt", @"file://log/stuff");
            path.Should().Be(@"file://log/stuff");
        }

        [TestMethod]
        public void MakeReleativePathShouldThrowWhenFirstArgNull()
        {
            Action act = () => FileExtensions.MakeRelativePath(null, @"file://somestuff");
            act.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName == "fromPath");
        }

        [TestMethod]
        public void MakeReleativePathShouldThrowWhenSecondeArgNull()
        {
            Action act = () => FileExtensions.MakeRelativePath(@"file://somestuff", null);
            act.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName == "toPath");
        }
    }
}