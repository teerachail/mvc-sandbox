
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HelloWorldMvc.Controllers
{
    public class HomeController 
    {
        [ActionContext]
        public ActionContext ActionContext { get; set; }

        public void Index()
        {
            ActionContext.HttpContext.Response.WriteAsync("Hello, World!");
        }
    }
}
