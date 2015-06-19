using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Sandbox.Web.Models
{
    public class PostedFiles
    {
        public IFormFile File1 { get; set; }

        public IFormFile File2 { get; set; }

        public IFormFile File3 { get; set; }
    }
}
