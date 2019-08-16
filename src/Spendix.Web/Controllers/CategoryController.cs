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
using Spendix.Web.Accessors;
using Spendix.Core.Accessors;
using Spendix.Core;

namespace Spendix.Web.Controllers
{
    [Authorize]
    [Route("Categories")]
    public class CategoryController : BaseController
    {
        private readonly SpendixDbContext spendixDbContext;
        private readonly BankAccountTransactionCategoryRepo bankAccountTransactionCategoryRepo;
        private readonly BankAccountTransactionSubCategoryRepo bankAccountTransactionSubCategoryRepo;
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public CategoryController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();
            bankAccountTransactionCategoryRepo = serviceProvider.GetService<BankAccountTransactionCategoryRepo>();
            bankAccountTransactionSubCategoryRepo = serviceProvider.GetService<BankAccountTransactionSubCategoryRepo>();
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Categories()
        {
            var categories = await bankAccountTransactionCategoryRepo.FindByUserAccountWithInvlucesAsync(loggedInUserAccountAccessor.GetLoggedInUserAccount());

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

        [HttpPost, Route("/api/Category/DeleteCategory/{categoryId}")]
        public async Task<IActionResult> DeleteCategroy(Guid categoryId)
        {
            var category = await bankAccountTransactionCategoryRepo.FindByIdAsync(categoryId);

            if (category != null)
            {
                var subCategories = await bankAccountTransactionSubCategoryRepo.FindByCategory(category);
                bankAccountTransactionSubCategoryRepo.DeleteSet(subCategories);
                bankAccountTransactionCategoryRepo.Delete(category);

                await spendixDbContext.SaveChangesAsync();
            }

            return Ok(new { });
        }

        [HttpPost, Route("/api/Category/DeleteSubCategory/{subCategoryId}")]
        public async Task<IActionResult> DeleteSubCategory(Guid subCategoryId)
        {
            var subCategory = await bankAccountTransactionSubCategoryRepo.FindByIdAsync(subCategoryId);

            if (subCategory != null)
            {
                bankAccountTransactionSubCategoryRepo.Delete(subCategory);

                await spendixDbContext.SaveChangesAsync();
            }

            return Ok(new { });
        }
    }
}