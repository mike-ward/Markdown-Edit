using System;
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
            var link = await service.UploadFileAsync(@"..\..\..\MarkdownEdit\logo.png");
            link.Should().StartWith("http://i.imgur.com/");
        }
    }
}
