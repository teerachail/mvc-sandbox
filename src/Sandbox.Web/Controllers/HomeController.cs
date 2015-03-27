using System;
using Microsoft.AspNet.Mvc;

namespace Sandbox.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Edit", "Products");
        }
    }
}