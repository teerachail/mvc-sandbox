
using Microsoft.AspNetCore.Mvc;

namespace LocalizedViews.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }
    }
}
