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
using Microsoft.AspNetCore.Mvc.Rendering;
using FluentDateTime;
using Spendix.Core.Entities;

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
        public async Task<IActionResult> Dashboard(Guid? bankAccountId, string range)
        {
            var bankAccounts = (await bankAccountRepo.FindByLoggedInUserAccountAsync()).OrderBy(x => x.Name);
            var ranges = await GetRanges();

            if (string.IsNullOrEmpty(range))
            {
                range = "CurrentMonth";
            }

            DateTime startDate;
            DateTime endDate;

            if (string.Equals(range, "CurrentMonth", StringComparison.InvariantCultureIgnoreCase))
            {
                startDate = DateTime.Today.FirstDayOfMonth();
                endDate = DateTime.Today.LastDayOfMonth();
            }
            else if (string.Equals(range, "CurrentMonth", StringComparison.InvariantCultureIgnoreCase))
            {
                startDate = DateTime.Today.FirstDayOfYear();
                endDate = DateTime.Today.LastDayOfYear();
            }
            else
            {
                var date = DateTime.Parse(range);
                startDate = date.FirstDayOfMonth();
                endDate = date.LastDayOfMonth();
            }

            BankAccount bankAccount = null;

            if (bankAccountId.HasValue)
            {
                bankAccount = bankAccounts.Single(x => x.BankAccountId == bankAccountId.Value);
            }

            var income = await bankAccountTransactionRepo.FindTotalIncomeByDateRange(startDate, endDate, bankAccount);
            var expenses = await bankAccountTransactionRepo.FindTotalExpensesByDateRange(startDate, endDate, bankAccount);
            var categoryAmounts = await bankAccountTransactionRepo.FindCategoryTotalsByDateRange(startDate, endDate, bankAccount);

            var vm = new DashboardViewModel
            {
                BankAccountSelectList = new SelectList(bankAccounts, "BankAccountId", "Name", bankAccountId),
                RangeSelectList = new SelectList(ranges, "Value", "Text", range),
                TotalIncome = income,
                TotalExpenses = expenses,
                ChartData = categoryAmounts.OrderBy(x => x.Amount).ToList()
            };

            return View(vm);
        }

        private async Task<List<SelectListItem>> GetRanges()
        {
            var monthsAndYears = await bankAccountTransactionRepo.FindTransactionMonthsAndYearsByLoggedInUserAccount();

            var ranges = new List<SelectListItem>
            {
                new SelectListItem{  Value = "CurrentMonth", Text = "Current Month" },
                new SelectListItem{  Value = "CurrentYear", Text = "Current Year" }
            };

            ranges.AddRange(monthsAndYears.Select(x => new SelectListItem
            {
                Value = new DateTime(x.Year, x.Month, 1).ToShortDateString(),
                Text = $"{x.Month}/{x.Year}"
            }).ToList());

            return ranges;
        }

        [HttpGet, Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}