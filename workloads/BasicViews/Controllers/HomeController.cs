
using Microsoft.AspNet.Mvc;

namespace BasicViews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }

        public IActionResult SuppressAntiforgery(Person person)
        {
            return View(person);
        }
    }
}
