
using BigModelBinding.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigModelBinding.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(EnrollmentService model)
        {
            if (ModelState.IsValid)
            {
                return Ok(model);
            }
            else
            {
                return HttpBadRequest(ModelState);
            }
        }
    }
}
