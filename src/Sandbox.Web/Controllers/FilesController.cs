using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Sandbox.Web.Models;

namespace Sandbox.Web.Controllers
{
    public class Files : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(PostedFiles files)
        {
            return View(files);
        }

        public IActionResult Many()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Many(List<IFormFile> files)
        {
            return View(files);
        }
    }
}
