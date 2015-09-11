
using System.Threading.Tasks;
using BigModelBinding.Models;
using Microsoft.AspNet.Mvc;

namespace BigModelBinding.Controllers
{
    public class HomeController : Controller
    {
        public static EnrollmentService DatabaseModel;

        public async Task<IActionResult> Index(EnrollmentService es)
        {
            if (ModelState.IsValid)
            {
                es = DatabaseModel;
                await TryUpdateModelAsync(es);
            }

            return View(es);
        }
    }
}
