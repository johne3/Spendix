using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core;
using Spendix.Core.Entities;
using Spendix.Core.Repos;
using Spendix.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Accessors;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("BankAccount")]
    public class BankAccountController : BaseController
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;
        private readonly SpendixDbContext spendixDbContext;
        private readonly BankAccountRepo bankAccountRepo;

        public BankAccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> SaveBankAccount(IFormCollection values)
        {
            var bankAccount = new BankAccount
            {
                Type = values["type"],
                Name = values["name"],
                OpeningBalance = decimal.Parse(values["openingBalance"]),
                UserAccountId = loggedInUserAccountAccessor.GetLoggedInUserAccountId()
            };

            bankAccountRepo.PrepareEntityForCommit(bankAccount);
            await spendixDbContext.SaveChangesAsync();

            return SetAlertMessageAndRedirect("Dashboard", "Home", "Bank Account Saved!", AlertMessageType.Success);
        }
    }
}