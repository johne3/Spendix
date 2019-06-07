using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Accessors;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Repos;
using Spendix.Core;
using Microsoft.AspNetCore.Http;
using Spendix.Core.Entities;
using Newtonsoft.Json.Linq;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("api/BankAccounts")]
    public class BankAccountApiController : BaseController
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;
        private readonly SpendixDbContext spendixDbContext;
        private readonly BankAccountRepo bankAccountRepo;

        public BankAccountApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetBankAccounts()
        {
            var bankAccounts = await bankAccountRepo.FindByLoggedInUserAccountAsync();

            return Json(bankAccounts.Select(x => new
            {
                x.BankAccountId,
                x.Name,
                x.Type,
                x.OpeningBalance,
                transactionsUrl = Url.Action("Transactions", "Transaction", new { x.BankAccountId })
            }).OrderBy(x => x.Name));
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> SaveBankAccount([FromBody]JObject json)
        {
            BankAccount bankAccount = null;

            if (!string.IsNullOrEmpty(json.Value<string>("bankAccountId")))
            {
                bankAccount = await bankAccountRepo.FindByIdAsync(Guid.Parse(json.Value<string>("bankAccountId")));
            }
            else
            {
                bankAccount = new BankAccount
                {
                    UserAccountId = loggedInUserAccountAccessor.GetLoggedInUserAccountId()
                };
            }

            bankAccount.Type = json.Value<string>("type");
            bankAccount.Name = json.Value<string>("name");
            bankAccount.OpeningBalance = json.Value<decimal>("openingBalance");

            bankAccountRepo.PrepareEntityForCommit(bankAccount);
            await spendixDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete, Route("")]
        public async Task<IActionResult> DeleteBankAccount([FromBody]Guid bankAccountId)
        {
            var bankAccount = await bankAccountRepo.FindByIdAsync(bankAccountId);

            bankAccountRepo.Delete(bankAccount);

            await spendixDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}