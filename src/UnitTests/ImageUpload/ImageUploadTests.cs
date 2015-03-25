using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ImageUpload
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task ImgurAnonymousUploadTestShouldReturnLink()
        {
            var service = new MarkdownEdit.ImageUpload.ImageUploadImgur();
            var imageBytes = File.ReadAllBytes(@"..\..\..\MarkdownEdit\logo.png");
            var link = await service.UploadBytesAsync(imageBytes);
            link.Should().StartWith("http://i.imgur.com/");
        }
    }
}
