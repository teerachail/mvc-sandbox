
using Microsoft.AspNet.Mvc;

namespace LargeStaticView
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
