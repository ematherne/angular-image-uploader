using System;
using System.Drawing;
using System.IO;

namespace Fnc.Upload.Core
{
    public class ImageHelper : IImageHelper
    {
        //TODO: Should really consider making these extension methods off of byte[] and Image

        /// <summary>
        /// Scale an image by the specified scale ratio
        /// </summary>
        /// <param name="image"></param>
        /// <param name="scaleRatio"></param>
        /// <returns>Newly scaled image</returns>
        public Image ScaleImage(Image image, double scaleRatio)
        {
            var newWidth = (int) (image.Width*scaleRatio);
            var newHeight = (int) (image.Height*scaleRatio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        /// <summary>
        /// Get a scale for image width based on the max allowed
        /// </summary>
        /// <param name="maxWidth"></param>
        /// <param name="imageWidth"></param>
        /// <returns></returns>
        public double GetScaleRatioWidth(double maxWidth, int imageWidth)
        {
            return (double) maxWidth/imageWidth;
        }

        /// <summary>
        /// Get a scale for image height based on max allowed
        /// </summary>
        /// <param name="maxHeight"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        public double GetScaleRatioHeight(double maxHeight, int imageHeight)
        {
            return (double) maxHeight/imageHeight;
        }

        /// <summary>
        /// Create / get an image type from a byte stream
        /// </summary>
        /// <param name="imageBytes">the byte stream to convert</param>
        /// <returns></returns>
        public Image GetImage(byte[] imageBytes)
        {
            using (var imageStream = new MemoryStream(imageBytes))
            {
                return Image.FromStream(imageStream);
            }
        }

        /// <summary>
        /// Resize an image according to max allowable pixel width and height
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public byte[] ResizeImage(Image image, double maxWidth, double maxHeight)
        {
            var newImage = ScaleImage(image,
                Math.Min(GetScaleRatioWidth(maxWidth, image.Width), GetScaleRatioHeight(maxHeight, image.Height)));
            return (byte[]) (new ImageConverter()).ConvertTo(newImage, typeof (byte[]));
        }

        /// <summary>
        /// Determine if an image needs to be resized based on width and height requirements
        /// </summary>
        /// <param name="image">The image to check</param>
        /// <param name="maxWidth">Max width dimension</param>
        /// <param name="maxHeight">Max height dimension</param>
        /// <returns>True if image should be resized</returns>
        public bool ResizeNeeded(Image image, double maxWidth, double maxHeight)
        {
            var ratioWidth = GetScaleRatioWidth(maxWidth, image.Width);
            var ratioHeight = GetScaleRatioHeight(maxHeight, image.Height);

            return (ratioWidth < 1 || ratioHeight < 1);
        }

//                public byte[] ResizeImage(MemoryStream imageStream, double maxWidth, double maxHeight)
//                {
//                    using (var image = Image.FromStream(imageStream))
//                    {
//                        var newImage = ScaleImage(image, Math.Min(maxWidth, maxHeight));
//                        return (byte[]) (new ImageConverter()).ConvertTo(newImage, typeof (byte[]));
//                    }
//                }

//        public byte[] ResizeImage(byte[] image, double maxWidth, double maxHeight)
//        {
//            using (var imageStream = new MemoryStream(image))
//            {
//                using (var newImage = Image.FromStream(imageStream))
//                {
//                    return ResizeImage(newImage, maxWidth, maxHeight);
//                }
//            }
//        }
    }
}