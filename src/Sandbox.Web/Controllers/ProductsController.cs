using System;
using Microsoft.AspNet.Mvc;
using Sandbox.Web.Models;

namespace Sandbox.Web.Controllers
{
    public class ProductsController : Controller
    {
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
    }
}