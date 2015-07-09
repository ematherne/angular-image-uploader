using System.Drawing;
using System.IO;

namespace Fnc.Upload.Core
{
    public interface IImageHelper
    {
        Image ScaleImage(Image image, double scaleRatio);
        double GetScaleRatioWidth(double maxWidth, int imageWidth);
        double GetScaleRatioHeight(double maxHeight, int imageHeight);
        Image GetImage(byte[] imageBytes);
//        byte[] ResizeImage(MemoryStream imageStream, double maxWidth, double maxHeight);
        byte[] ResizeImage(Image image, double maxWidth, double maxHeight);
//        byte[] ResizeImage(byte[] image, double maxWidth, double maxHeight);
        bool ResizeNeeded(Image image, double maxWidth, double maxHeight);
    }
}