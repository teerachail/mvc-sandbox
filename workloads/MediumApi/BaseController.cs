using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace MediumApi
{
    public class BaseController
    {
        [ActionContext]
        public ActionContext ActionContext { get; set; }

        public ModelStateDictionary ModelState => ActionContext.ModelState;
    }
}
