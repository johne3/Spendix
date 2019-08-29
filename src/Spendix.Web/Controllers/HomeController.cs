using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Accessors;
using Spendix.Web.ViewModels;
using Spendix.Web.ViewModels.Home;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Repos;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("")]
    public class HomeController : BaseController
    {
        private readonly BankAccountRepo bankAccountRepo;
        private readonly BankAccountTransactionRepo bankAccountTransactionRepo;

        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
            bankAccountTransactionRepo = serviceProvider.GetService<BankAccountTransactionRepo>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Dashboard()
        {
            var bankAccounts = await bankAccountRepo.FindByLoggedInUserAccountAsync();

            var monthsAndYears = await bankAccountTransactionRepo.FindTransactionMonthsAndYearsByLoggedInUserAccount();

            var vm = new DashboardViewModel();

            return View(vm);
        }

        [HttpGet, Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}