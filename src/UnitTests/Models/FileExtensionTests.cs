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
            var path = FileExtensions.MakeRelativePath(@"file://mystuff\stuff\assets\test.txt", @"file://mystuff\stuff\test.txt");
            path.Should().Be(@"../test.txt");
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