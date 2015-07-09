using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Fnc.Upload.Models;

namespace Fnc.Upload.Core
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly string filePath;
        private readonly IImageHelper imageHelper;

        // TODO: Put these in config file
        private const double maxWidth = 500; 
        private const double maxHeight = 700;

//        public ImageProcessor()
//        {
//            this.filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
//                ConfigurationManager.AppSettings["filePath"]);
//        }

        public ImageProcessor(IImageHelper imageHelper)
        {
            this.imageHelper = imageHelper;
            this.filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ConfigurationManager.AppSettings["filePath"]);
        }

        /// <summary>
        /// We're going to want this if we track the files in a db
        /// </summary>
        /// <param name="id">Image Id</param>
        /// <returns></returns>
        public UploadImage LoadImage(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stub to return all images 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UploadImage> LoadImages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save an Image and resize if necessary
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <returns></returns>
        public async Task<UploadImage> SaveAndResizeImage(UploadImage uploadImage)
        {
            try
            {
                var uploadPath = Path.Combine(this.filePath, Path.GetFileName(uploadImage.fileName));

                using (var fs = new FileStream(uploadPath, FileMode.Create))
                {
                    await fs.WriteAsync(uploadImage.file, 0, uploadImage.file.Length);

                    var resizeImage = imageHelper.GetImage(uploadImage.file);

                    if (imageHelper.ResizeNeeded(resizeImage, maxWidth, maxHeight))
                    {
                        var resized = imageHelper.ResizeImage(resizeImage, maxWidth, maxHeight);
                        using (var resizedFs = new FileStream(Path.Combine(this.filePath, Path.GetFileName(GetResizedFileName(uploadImage.fileName))), FileMode.Create))
                        {
                            await resizedFs.WriteAsync(resized, 0, resized.Length);

                            // Set the original image byte stream to the new resized value
                            uploadImage.file = resized;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Error saving file: " + exception.Message);
            }

            return uploadImage;
        }

        /// <summary>
        /// Generate a new file name for a resized image
        /// </summary>
        /// <param name="originalFileName">Filename of unscaled image</param>
        /// <returns></returns>
        public string GetResizedFileName(string originalFileName)
        {
            return Path.GetFileNameWithoutExtension(originalFileName) + "-resized" + Path.GetExtension(originalFileName);
        }

        /// <summary>
        /// Resize an image in the memory stream
        /// </summary>
        /// <param name="imageStream">A memory stream with the image to resize</param>
        /// <returns>Resized image or null if no resizing needed</returns>
        public byte[] ResizeImage(MemoryStream imageStream)
        {
            using (var image = Image.FromStream(imageStream))
            {
                var ratioWidth = (double) maxWidth/image.Width;
                var ratioHeight = (double) maxHeight/image.Height;

                if (ratioWidth < 1 || ratioHeight < 1)
                {
                    var newImage = ScaleImage(image, Math.Min(ratioWidth, ratioHeight));
                    return (byte[]) (new ImageConverter()).ConvertTo(newImage, typeof (byte[]));
                }

                return null;
            }
        }

        /// <summary>
        /// Scale an image by the specified scale ratio
        /// </summary>
        /// <param name="image"></param>
        /// <param name="scaleRatio"></param>
        /// <returns>Newly scaled image</returns>
        private Image ScaleImage(Image image, double scaleRatio)
        {
            var newWidth = (int) (image.Width*scaleRatio);
            var newHeight = (int) (image.Height*scaleRatio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }
    }
}
