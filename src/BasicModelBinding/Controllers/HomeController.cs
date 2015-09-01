
using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;

namespace BasicModelBinding
{
    public class HomeController 
    {
        [ActionContext]
        public ActionContext ActionContext { get; set; }

        public async Task Index()
        {
            await ActionContext.HttpContext.Response.WriteAsync("name=Joey&age=15&birthdate=5/5/2008");
        }

        [HttpPost]
        public async Task Index(Person person)
        {
            if (ActionContext.ModelState.IsValid)
            {
                await ActionContext.HttpContext.Response.WriteAsync("OK");
            }
            else
            {
                ActionContext.HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpPost]
        public async Task Manual()
        {
            var person = new Person();

            var form = await ActionContext.HttpContext.Request.ReadFormAsync();
            person.Name = form["name"];
            person.Age = int.Parse(form["age"]);
            person.BirthDate = DateTime.Parse(form["birthdate"]);
            
            if (ActionContext.ModelState.IsValid)
            {
                await ActionContext.HttpContext.Response.WriteAsync("OK");
            }
            else
            {
                ActionContext.HttpContext.Response.StatusCode = 400;
            }
        }
    }
}
