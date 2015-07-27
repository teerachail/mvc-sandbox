
using Microsoft.AspNet.Mvc;

namespace BasicViews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Person person)
        {
            return View();
        }
    }
}
