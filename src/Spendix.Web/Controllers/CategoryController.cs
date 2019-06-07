using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("Categories")]
    public class CategoryController : BaseController
    {
        public CategoryController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet, Route("")]
        public IActionResult Items()
        {
            return View();
        }
    }
}