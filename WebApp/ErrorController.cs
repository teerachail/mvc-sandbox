using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            var feature = HttpContext.Features.Get<IStatusCodePagesFeature>();
            return View(HttpContext.Response.StatusCode);
        }

        public IActionResult Exception()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View(feature.Error);
        }

        public IActionResult ViewCompilationException()
        {
            return View();
        }

        public IActionResult ThrowHandled()
        {
            throw null;
        }

        public IActionResult ThrowingHandler()
        {
            throw new Exception("Exception thrown by handler");
        }

        public IActionResult ThrowingView()
        {
            return View();
        }

        public IActionResult Throw()
        {
            throw new Exception("Exception thrown by action.");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is NullReferenceException)
            {
                context.Result = new ObjectResult(new { error = context.Exception.Message })
                {
                    StatusCode = 500,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
