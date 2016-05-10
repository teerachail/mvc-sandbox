using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [EnableCors("all")]
    [Route("[controller]")]
    public class ItemsController
    {
        [HttpGet]
        public IActionResult GetItems()
        {
            return new ObjectResult(new[]
            {
                new { id = 0, name = "joey" },
            });
        }

        [HttpPost]
        public IActionResult PostItem()
        {
            return new ObjectResult(new[]
            {
                new { id = 1, name = "billy" },
            });
        }

        [HttpPut]
        [EnableCors("specific")]
        public IActionResult PutItem()
        {
            return new ObjectResult(new[]
            {
                new { id = 1, name = "billy" },
            });
        }
    }
}
