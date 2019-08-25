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
using System.Text.RegularExpressions;
using Spendix.Core.Entities;

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
            var categories = await bankAccountTransactionCategoryRepo.FindByUserAccountWithIncludesAsync(loggedInUserAccountAccessor.GetLoggedInUserAccount());

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
            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();
            var categories = await bankAccountTransactionCategoryRepo.FindByUserAccountAsync(userAccount);

            var paymentNumbers = GetNumberFromForm(form, "PaymentCategory_", "_CategoryId");
            var depositNumbers = GetNumberFromForm(form, "DepositCategory_", "_CategoryId");

            PopulateCategoriesForSave(form, paymentNumbers, TransactionTypes.Payment, categories);
            PopulateCategoriesForSave(form, depositNumbers, TransactionTypes.Deposit, categories);

            await spendixDbContext.SaveChangesAsync();

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

        private List<int> GetNumberFromForm(IFormCollection form, string prefix, string suffix)
        {
            var numberRegex = new Regex($"{prefix}(?<Number>\\d*){suffix}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

            var numbers = new List<int>();

            foreach (var key in form.Keys)
            {
                var match = numberRegex.Match(key.ToString());

                if (match.Success)
                {
                    numbers.Add(int.Parse(match.Groups["Number"].Value));
                }
            }

            return numbers;
        }

        private void PopulateCategoriesForSave(IFormCollection form, List<int> numbers, string transactionType, List<BankAccountTransactionCategory> categories)
        {
            foreach (var number in numbers)
            {
                var inputNamePrefix = $"{transactionType}Category_{number}_";
                var categoryIdKey = $"{inputNamePrefix}CategoryId";
                var categoryNameKey = $"{inputNamePrefix}Name";

                var categoryId = form[categoryIdKey];

                BankAccountTransactionCategory category;

                if (string.IsNullOrEmpty(categoryId))
                {
                    category = new BankAccountTransactionCategory
                    {
                        UserAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount(),
                        TransactionType = transactionType,
                        BankAccountTransactionSubCategories = new List<BankAccountTransactionSubCategory>()
                    };
                }
                else
                {
                    category = categories.Single(x => x.BankAccountTransactionCategoryId == Guid.Parse(categoryId));
                }

                if (!string.IsNullOrEmpty(form[categoryNameKey]))
                {
                    category.Name = form[categoryNameKey];

                    bankAccountTransactionCategoryRepo.PrepareEntityForCommit(category);

                    PopulateSubCategoriesForSave(form, inputNamePrefix, category);
                }
            }
        }

        private void PopulateSubCategoriesForSave(IFormCollection form, string inputNamePrefix, BankAccountTransactionCategory category)
        {
            //Populate sub categories - PaymentCategory_0_SubCategory_0_SubCategoryId
            var subCategoryNumbers = GetNumberFromForm(form, $"{inputNamePrefix}SubCategory_", "_SubCategoryId");

            foreach (var subCategoryNumber in subCategoryNumbers)
            {
                var subCategoryIdKey = $"{inputNamePrefix}SubCategory_{subCategoryNumber}_SubCategoryId";
                var subCtaegoryNameKey = $"{inputNamePrefix}SubCategory_{subCategoryNumber}_Name";

                var subCategoryId = form[subCategoryIdKey];

                BankAccountTransactionSubCategory subCategory;

                if (string.IsNullOrEmpty(subCategoryId))
                {
                    subCategory = new BankAccountTransactionSubCategory
                    {
                        BankAccountTransactionCategory = category
                    };
                }
                else
                {
                    subCategory = category.BankAccountTransactionSubCategories.Single(x => x.BankAccountTransactionSubCategoryId == Guid.Parse(subCategoryId));
                }

                if (!string.IsNullOrEmpty(form[subCtaegoryNameKey]))
                {
                    subCategory.Name = form[subCtaegoryNameKey];

                    bankAccountTransactionSubCategoryRepo.PrepareEntityForCommit(subCategory);
                }
            }
        }
    }
}