using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Accessors;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Repos;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("api/BankAccounts")]
    public class BankAccountApiController : BaseController
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;
        private readonly BankAccountRepo bankAccountRepo;

        public BankAccountApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get()
        {
            var bankAccounts = await bankAccountRepo.FindByLoggedInUserAccountAsync();

            return Json(bankAccounts.Select(x => new
            {
                x.BankAccountId,
                x.Name,
                detailUrl = Url.Action("Detail", "BankAccount", new { })
            }));
        }
    }
}