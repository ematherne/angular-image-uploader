using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Fnc.Upload.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fnc.Upload.Tests.Controllers
{
    [TestClass]
    public class ImageHelperTest
    {
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
        public void ResizeImageFromImage()
        {
            CreateImage("testImage.png", 900, 900, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);
            Image image;

            using (var imageStream = new MemoryStream(file))
            {
                image = Image.FromStream(imageStream);
            }

            var helper = new ImageHelper();

            var result = helper.ResizeImage(image, 500, 700);

            Assert.AreNotEqual(file.Length, result.Length);
            DeleteImage("testImage.png");
        }

        [TestMethod]
        public void ResizeNeededReturnsTrue()
        {
            CreateImage("testImage.png", 900, 900, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);
            Image image;

            using (var imageStream = new MemoryStream(file))
            {
                image = Image.FromStream(imageStream);
            }

            var helper = new ImageHelper();
            var result = helper.ResizeNeeded(image, 500, 700);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ResizeNeededReturnsFalse()
        {
            CreateImage("testImage.png", 500, 700, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);
            Image image;

            using (var imageStream = new MemoryStream(file))
            {
                image = Image.FromStream(imageStream);
            }

            var helper = new ImageHelper();
            var result = helper.ResizeNeeded(image, 500, 700);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ScaleImageReturnsNewImage()
        {
            CreateImage("testImage.png", 900, 900, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);
            Image image;

            using (var imageStream = new MemoryStream(file))
            {
                image = Image.FromStream(imageStream);
            }

            var scaleRatio = Math.Min((double) 500 / image.Width,
                (double) 700/ image.Height);

            var helper = new ImageHelper();
            var result = helper.ScaleImage(image, scaleRatio);

            Assert.AreNotEqual(image.Size, result.Size);
        }

        [TestMethod]
        public void GetImageReturnsImage()
        {
            CreateImage("testImage.png", 500, 700, ImageFormat.Png);
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"], Path.GetFileName("testImage.png"));

            var file = File.ReadAllBytes(filepath);

            Image expected;

            using (var imageStream = new MemoryStream(file))
            {
                expected = Image.FromStream(imageStream);
            }

            var result = new ImageHelper().GetImage(file);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Size, result.Size);
        }
    }
}
