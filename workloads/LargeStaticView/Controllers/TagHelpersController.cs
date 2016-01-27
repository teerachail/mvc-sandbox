
using Microsoft.AspNetCore.Mvc;

namespace LargeStaticView
{
    public class TagHelpersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
