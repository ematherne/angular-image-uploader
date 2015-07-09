using System.Collections.Generic;
using System.Threading.Tasks;
using Fnc.Upload.Models;

namespace Fnc.Upload.Core
{
    public interface IImageProcessor
    {
        UploadImage LoadImage(int id);
        IEnumerable<UploadImage> LoadImages();
        Task<UploadImage> SaveAndResizeImage(UploadImage image);
    }
}