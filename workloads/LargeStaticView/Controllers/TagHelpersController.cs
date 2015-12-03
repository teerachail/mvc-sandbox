
using Microsoft.AspNet.Mvc;

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
