using Microsoft.AspNet.Mvc;

namespace BigViews
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(BigViews.User.StaticUsers);
        }

        public IActionResult Edit(User user)
        {
            var result = new ContentResult();
            result.Content = user.FirstName;
            return result;
        }
    }
}
