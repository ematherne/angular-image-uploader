using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Fnc.Upload.Core;
using Fnc.Upload.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Fnc.Upload.Tests.Controllers
{
    [TestClass]
    public class ImagesProcessorTest
    {
        private bool FileExists(string fileName)
        {
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName(fileName));

            return File.Exists(filepath);
        }

        private void CreateImage(string fileName, int width, int height, ImageFormat imageFormat)
        {
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName(fileName));
            
            var newImage = new Bitmap(width, height);
            
            newImage.Save(filepath, imageFormat);
        }

        private void DeleteImage(string fileName)
        {
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName(fileName));
            File.Delete(filepath);
        }

        [TestMethod]
        [Ignore]
        public void LoadImageReturnsImage()
        {
            var image = new ImageProcessor(new ImageHelper()).LoadImage(1);
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public async Task SaveImage()
        {
            CreateImage("testImage.png", 500, 500, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var uploadImage = new UploadImage()
            {
                file = File.ReadAllBytes(filepath),
                fileName = "testImage2.png"
            };

            var processor = new ImageProcessor(new ImageHelper());
            var image = await processor.SaveAndResizeImage(uploadImage);

            Assert.AreEqual(image, uploadImage);
            Assert.IsTrue(FileExists(image.fileName));

            DeleteImage("testImage.png");
            DeleteImage("testImage2.png");
        }

        [TestMethod]
        public void GetResizedFileName()
        {
            var fileName = "image.png";
            var processor = new ImageProcessor(new ImageHelper());
            var result = processor.GetResizedFileName(fileName);

            Assert.AreEqual("image-resized.png", result);
        }

        [TestMethod]
        public void ResizeImage()
        {
            CreateImage("testImage.png", 900, 900, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);

            byte[] result;

            var processor = new ImageProcessor(new ImageHelper());

            using (var ms = new MemoryStream(file))
            {
                result = processor.ResizeImage(ms);
            }
    
            Assert.AreNotEqual(file.Length, result.Length);
            DeleteImage("testImage.png");
        }

        [TestMethod]
        public async Task SaveImageReturnsResized()
        {
            CreateImage("testImage.png", 900, 900, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var uploadImage = new UploadImage()
            {
                file = File.ReadAllBytes(filepath),
                fileName = "testImage2.png"
            };

            var processor = new ImageProcessor(new ImageHelper());
            var image = await processor.SaveAndResizeImage(uploadImage);

            Assert.AreEqual(image, uploadImage);
            Assert.IsTrue(FileExists(image.fileName));

            DeleteImage("testImage.png");
            DeleteImage("testImage2.png");
        }

    }
}
