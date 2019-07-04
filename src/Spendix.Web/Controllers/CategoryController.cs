using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Repos;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Web.ViewModels.Category;
using Spendix.Core.Constants;
using Microsoft.AspNetCore.Http;
using Spendix.Web.Models;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("Categories")]
    public class CategoryController : BaseController
    {
        private readonly BankAccountTransactionCategoryRepo bankAccountTransactionCategoryRepo;

        public CategoryController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            bankAccountTransactionCategoryRepo = serviceProvider.GetService<BankAccountTransactionCategoryRepo>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Categories()
        {
            var categories = await bankAccountTransactionCategoryRepo.FindByActiveAsync();

            var vm = new CategoriesViewModel
            {
                PaymentCategories = categories.Where(x => x.TransactionType == TransactionTypes.Payment).OrderBy(x => x.Name).ToList(),
                DepositCategories = categories.Where(x => x.TransactionType == TransactionTypes.Deposit).OrderBy(x => x.Name).ToList()
            };

            return View(vm);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> Categories(IFormCollection form)
        {
            return SetAlertMessageAndRedirect("Categories", "Category", "Categories saved!", AlertMessageType.Success);
        }
    }
}