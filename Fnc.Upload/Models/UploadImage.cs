using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Fnc.Upload.Models
{
    public class UploadImage
    {
        public string fileName { get; set; }
        public byte[] file { get; set; } 

        // These fields exist if we will be saving the location and / or image to DB at a later iteration
        public int id { get; set; }
        public string filepath { get; set; }
        public string thumbnailPath { get; set; }

    }
}