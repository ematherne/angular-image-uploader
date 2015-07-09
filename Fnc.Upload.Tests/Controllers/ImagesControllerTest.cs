using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using System.Web.UI.WebControls;
using Fnc.Upload.Controllers;
using Fnc.Upload.Core;
using Fnc.Upload.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fnc.Upload.Tests.Controllers
{
    [TestClass]
    public class ImagesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            var processor = new Mock<IImageProcessor>();
            processor.Setup(x => x.LoadImages()).Returns(new List<UploadImage>()
            {
                new UploadImage()
                {
                    id = 1,
                    filepath = "this/is/a/filepath",
                    thumbnailPath = "this/is/a/thumbnail/path",
                    fileName = "test image"
                },
                new UploadImage()
                {
                    id = 2,
                    filepath = "this/is/a/filepath",
                    thumbnailPath = "this/is/a/thumbnail/path",
                    fileName = "another image"
                }
            });

            var controller = new ImagesController(processor.Object);

            var result = controller.Get();
            var contentResult = result as OkNegotiatedContentResult<IEnumerable<UploadImage>>;

            Assert.IsNotNull(result);   
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(2, contentResult.Content.Last().id);

        }

        [TestMethod]
        public void GetById()
        {
            var processor = new Mock<IImageProcessor>();
            processor.Setup(x => x.LoadImage(1)).Returns(new UploadImage()
            {
                id = 1,
                filepath = "this/is/a/filepath",
                thumbnailPath = "this/is/a/thumbnail/path",
                fileName = "test image"
            });

            var controller = new ImagesController(processor.Object);

            var result = controller.Get("1");
            var contentResult = result as OkNegotiatedContentResult<UploadImage>;

            Assert.AreEqual(1, contentResult.Content.id);
        }

        [TestMethod]
        public void GetById_NotFound()
        {
            var processor = new Mock<IImageProcessor>();
            processor.Setup(x => x.LoadImage(It.IsAny<int>())).Returns((UploadImage) null);
            var controller = new ImagesController(processor.Object);

            var result = controller.Get("2");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Post()
        {
            // Mock dependency
            var processor = new Mock<IImageProcessor>();

            var file = new UTF8Encoding(true).GetBytes("This is some text in the file blah.");

            var uploadImage = new UploadImage()
            {
                filepath = "a/filepath",
                id = 1,
                thumbnailPath = "a/filepath/thumbnails",
                file = file,
                fileName = "testImage"
            };
            processor.Setup(x => x.SaveAndResizeImage(It.IsAny<UploadImage>())).ReturnsAsync(uploadImage);

            // Setup content headers for POST
            var content = GetContentHeader(uploadImage);

            // Setup controller
            var controller = new ImagesController(processor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            controller.Request.Content = content;

            // Post file
            var result = controller.Post();
            var contentResult = await result as OkNegotiatedContentResult<byte[]>;

            Assert.AreEqual(uploadImage.file, contentResult.Content);
        }

        private MultipartFormDataContent GetContentHeader(UploadImage uploadImage)
        {
            var fileContent = new ByteArrayContent(uploadImage.file);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Name = "attachment",
                FileName = uploadImage.fileName
            };

            return new MultipartFormDataContent { fileContent };
        }
    }
}
