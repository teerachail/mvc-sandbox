
using Microsoft.AspNet.Mvc;

namespace BasicViews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }
    }
}
