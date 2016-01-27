
using Microsoft.AspNetCore.Mvc;

namespace BasicViews.Controllers
{
    public class TagHelpers : Controller
    {
        public IActionResult Index(Person person)
        {
            return View(person);
        }
    }
}
