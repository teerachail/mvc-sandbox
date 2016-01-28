
using Microsoft.AspNetCore.Mvc;

namespace LocalizedViews.Controllers
{
    public class TagHelpers : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }
    }
}
