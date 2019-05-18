using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spendix.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController(IServiceProvider serviceProvider)
        {
        }
    }
}
