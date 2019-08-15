using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spendix.Core.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Spendix.Core.Constants;
using Spendix.Web.ViewModels.SubCategory;
using Spendix.Web.Accessors;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("SubCategories")]
    public class SubCategoryController : BaseController
    {
        private readonly BankAccountTransactionCategoryRepo bankAccountTransactionCategoryRepo;
        private readonly BankAccountTransactionSubCategoryRepo bankAccountTransactionSubCategoryRepo;
        private LoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public SubCategoryController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            bankAccountTransactionCategoryRepo = serviceProvider.GetService<BankAccountTransactionCategoryRepo>();
            bankAccountTransactionSubCategoryRepo = serviceProvider.GetService<BankAccountTransactionSubCategoryRepo>();
            loggedInUserAccountAccessor = serviceProvider.GetService<LoggedInUserAccountAccessor>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> SubCategories()
        {
            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();
            var categories = await bankAccountTransactionCategoryRepo.FindByUserAccountAsync(userAccount);
            var subCategories = await bankAccountTransactionSubCategoryRepo.FindByUserAccountWithIncludesAsync(userAccount);

            var vm = new SubCategoriesViewModel
            {
                PaymentCategories = categories.Where(x => x.TransactionType == TransactionTypes.Payment).OrderBy(x => x.Name).ToList(),
                DepositCategories = categories.Where(x => x.TransactionType == TransactionTypes.Deposit).OrderBy(x => x.Name).ToList(),
                PaymentSubCategories = subCategories.Where(x => x.BankAccountTransactionCategory.TransactionType == TransactionTypes.Payment).OrderBy(x => x.BankAccountTransactionCategory.Name).ToList(),
                DepositSubCategories = subCategories.Where(x => x.BankAccountTransactionCategory.TransactionType == TransactionTypes.Deposit).OrderBy(x => x.BankAccountTransactionCategory.Name).ToList()
            };

            return View(vm);
        }
    }
}