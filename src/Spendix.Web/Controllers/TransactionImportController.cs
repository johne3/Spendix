using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Spendix.Web.Controllers
{
    [Route("Transactions")]
    [Authorize]
    public class TransactionImportController : BaseController
    {
        public TransactionImportController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet, Route("Import")]
        public IActionResult Import()
        {
            return View();
        }
    }
}