using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core;
using Spendix.Core.Accessors;
using Spendix.Core.Repos;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Web.ViewModels.BankAccountTransactions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Constants;
using Spendix.Core.Entities;

namespace Spendix.Web.Controllers
{
    [Route("Transactions")]
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;
        private readonly BankAccountRepo bankAccountRepo;
        private readonly BankAccountTransactionRepo bankAccountTransactionRepo;
        private readonly BankAccountTransactionCategoryRepo bankAccountTransactionCategoryRepo;
        private readonly BankAccountTransactionSubCategoryRepo bankAccountTransactionSubCategoryRepo;

        private readonly SpendixDbContext spendixDbContext;

        public TransactionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
            bankAccountTransactionRepo = serviceProvider.GetService<BankAccountTransactionRepo>();
            bankAccountTransactionCategoryRepo = serviceProvider.GetService<BankAccountTransactionCategoryRepo>();
            bankAccountTransactionSubCategoryRepo = serviceProvider.GetService<BankAccountTransactionSubCategoryRepo>();

            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();
        }

        [HttpGet, Route("{bankAccountId}/{bankAccountTransactionId?}")]
        public async Task<IActionResult> Transactions(Guid bankAccountId, Guid? bankAccountTransactionId = null)
        {
            var bankAccount = await bankAccountRepo.FindByIdAsync(bankAccountId);
            var transactionTypes = new List<string> { TransactionTypes.Payment, TransactionTypes.Deposit };
            var transactions = (await bankAccountTransactionRepo.FindByBankAccountAsync(bankAccount))
                .OrderBy(x => x.TransactionDate)
                .ThenBy(x => x.TransactionEnteredDateUtc)
                .ToList();

            var balances = new List<(BankAccountTransaction transaction, decimal balance)>();
            var currentBalance = bankAccount.OpeningBalance;

            foreach (var transaction in transactions)
            {
                currentBalance += transaction.Amount;

                balances.Add((transaction, currentBalance));
            }

            var vm = new TransactionsViewModel
            {
                TransactionTypeSelectList = new SelectList(transactionTypes),
                BankAccount = bankAccount,
                Transactions = transactions,
                TransactionBalances = balances.OrderByDescending(x => x.transaction.TransactionDate).ThenByDescending(x => x.transaction.CreateDateUtc).ToList()
            };

            if (bankAccountTransactionId.HasValue)
            {
                var bankAccountTransaction = await bankAccountTransactionRepo.FindByIdAsync(bankAccountTransactionId.Value);

                if (bankAccountTransaction == null)
                {
                    throw new Exception("Bank Account Transaction not found by id.");
                }

                vm.SelectedBankAccountTransaction = bankAccountTransaction;
            }

            return View(vm);
        }

        [HttpPost, Route("Transaction/{bankAccountId}")]
        public async Task<IActionResult> SaveTransaction(Guid? bankAccountId, IFormCollection values)
        {
            var bankAccount = await bankAccountRepo.FindByIdAsync(bankAccountId);

            if (bankAccount == null)
            {
                throw new Exception("Bank Account not found by id.");
            }

            var bankAccountTransactionCategory = await bankAccountTransactionCategoryRepo.FindByIdAsync(Guid.Parse(values["Category"]));

            BankAccountTransaction bankAccountTransaction = null;

            if (!string.IsNullOrEmpty(values["BankAccountTransactionId"]))
            {
                bankAccountTransaction = await bankAccountTransactionRepo.FindByIdAsync(Guid.Parse(values["BankAccountTransactionId"]));
            }
            else
            {
                bankAccountTransaction = new BankAccountTransaction
                {
                    BankAccountId = bankAccount.BankAccountId
                };
            }

            bankAccountTransaction.TransactionDate = DateTime.Parse(values["Date"]);
            bankAccountTransaction.TransactionEnteredDateUtc = DateTime.UtcNow;
            bankAccountTransaction.Payee = values["Payee"];
            bankAccountTransaction.BankAccountTransactionCategoryId = bankAccountTransactionCategory.BankAccountTransactionCategoryId;

            if (string.IsNullOrEmpty(values["SubCategory"]))
            {
                bankAccountTransaction.BankAccountTransactionSubCategoryId = null;
            }
            else
            {
                bankAccountTransaction.BankAccountTransactionSubCategoryId = Guid.Parse(values["SubCategory"]);
            }

            if (string.Equals(bankAccountTransactionCategory.TransactionType, TransactionTypes.Payment))
            {
                bankAccountTransaction.Amount = decimal.Negate(decimal.Parse(values["Amount"]));
            }
            else
            {
                bankAccountTransaction.Amount = decimal.Parse(values["Amount"]);
            }

            bankAccountTransactionRepo.PrepareEntityForCommit(bankAccountTransaction);

            await spendixDbContext.SaveChangesAsync();

            return RedirectToAction("Transactions", "Transaction", new { bankAccountId = bankAccount.BankAccountId });
        }

        [HttpGet, Route("/api/TransactionCategories/{transactionType}")]
        public async Task<IActionResult> TransactionCategories(string transactionType)
        {
            var categories = (await bankAccountTransactionCategoryRepo.FindByUserAccountAndTransactionType(loggedInUserAccountAccessor.GetLoggedInUserAccount(), transactionType)).OrderBy(x => x.Name);

            return Ok(new
            {
                Categories = categories.Select(x => new
                {
                    x.BankAccountTransactionCategoryId,
                    x.Name
                })
            });
        }

        [HttpGet, Route("/api/TransactionSubCategories/{categoryId}")]
        public async Task<IActionResult> TransactionSubCategories(Guid categoryId)
        {
            var category = await bankAccountTransactionCategoryRepo.FindByIdWithIncludesAsync(categoryId);

            if (category == null)
            {
                throw new Exception("Unable to find BankAccountTransactionCategory by id.");
            }

            return Ok(new
            {
                SubCategories = category.BankAccountTransactionSubCategories.OrderBy(x => x.Name).Select(x => new
                {
                    x.BankAccountTransactionSubCategoryId,
                    x.Name
                })
            });
        }
    }
}