using System;
using Microsoft.AspNet.Mvc;

namespace MediumApi.Controllers
{
    [Route("/user")]
    public class UserController : BaseController
    {
        [HttpPost]
        public IActionResult CreateUser()
        {
            throw new NotImplementedException();
        }

        [HttpPost("createWithArray")]
        public IActionResult CreateUserWithArray()
        {
            throw new NotImplementedException();
        }

        [HttpPost("createWithList")]
        public IActionResult CreateUserWithList()
        {
            throw new NotImplementedException();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            throw new NotImplementedException();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{username}")]
        public IActionResult GetUser(string username)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{username}")]
        public IActionResult UpdateUser(string username)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{username}")]
        public IActionResult DeleteUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
