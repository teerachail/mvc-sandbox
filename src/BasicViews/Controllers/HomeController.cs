
using Microsoft.AspNet.Mvc;

namespace BasicViews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new Person());
        }

        [HttpPost]
        public IActionResult Index(Person person)
        {
            return View();
        }
    }
}
