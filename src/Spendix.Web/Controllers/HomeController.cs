using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spendix.Web.ViewModels;

namespace Spendix.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet, Route("")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet, Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
