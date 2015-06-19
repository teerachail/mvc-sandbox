using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using Sandbox.Web.Models;

namespace Sandbox.Web.Controllers
{
    public class ProductsController : Controller
    {
        [FromServices]
        public IJsonHelper JsonHelper { get; set; }

        public IActionResult Edit()
        {
            var model = new Product()
            {
                Id = 1,
                Name = "Bald-Head Wax",
                Price = 19.95m,
            };
            return View(model);
        }


        public IActionResult Get()
        {
            var model = new Product()
            {
                Id = 1,
                Name = "Bald-Head Wax",
                Price = 19.95m,
            };

            return Json(model, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
            });
        }
    }
}