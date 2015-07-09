using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Fnc.Upload.Core;
using Fnc.Upload.Filters;
using Fnc.Upload.Models;

namespace Fnc.Upload.Controllers
{
    public class ImagesController : ApiController
    {
        private readonly IImageProcessor processor;

        public ImagesController(IImageProcessor processor)
        {
            this.processor = processor;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            // This method is more for later to retrieve the image from a db
            IEnumerable<UploadImage> images;
            try
            {
                images = this.processor.LoadImages();
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }

            return Ok(images);
        }

        [HttpGet]
        public IHttpActionResult Get(string Id)
        {
            // This method is more for later to retrieve the image from a db 
            UploadImage image;
            try
            {
                image = this.processor.LoadImage(Int32.Parse(Id));
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }

            return image != null ? (IHttpActionResult) Ok(image) : NotFound();
        }

        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<IHttpActionResult> Post()
        {
            var image = new UploadImage();
            try
            {
                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.Contents)
                {
                    // Seems kind of hackish as a method for determining where the file name is in the content stream
                    if (file.Headers.ContentDisposition.FileName != null)
                    {
                        // Get file specifics and save 
                        image.fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                        image.file = await file.ReadAsByteArrayAsync();

                        image = await this.processor.SaveAndResizeImage(image);
                    }
                }

            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
            // Technically we could return the upload image json if we needed that client side
            return Ok(image.file);
        }
    }
}
