using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MarkdownEdit.ImageUpload;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.ImageUpload
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod, Ignore]
        public async Task ImgurAnonymousUploadTestShouldReturnLink()
        {
            var service = new ImageUploadImgur();
            var imageBytes = File.ReadAllBytes(@"MarkdownEdit\logo.png");
            var link = await service.UploadBytesAsync(imageBytes);
            link.Should().StartWith("http://i.imgur.com/");
        }
    }
}