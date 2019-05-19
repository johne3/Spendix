using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Accessors;
using Microsoft.Extensions.DependencyInjection;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("api/BankAccounts")]
    public class BankAccountApiController : BaseController
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public BankAccountApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get()
        {
            var dummyBankAccounts = new List<dynamic>
            {
                new { bankAccountId = Guid.NewGuid(), name = "Checking", detailUrl = $"/BankAccount/{Guid.NewGuid()}" },
                new { bankAccountId = Guid.NewGuid(), name = "Savings", detailUrl = $"/BankAccount/{Guid.NewGuid()}" }
            };

            return Json(dummyBankAccounts);
        }
    }
}