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
using System.Security;

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
            var bankAccounts = (await bankAccountRepo.FindByLoggedInUserAccountAsync())
                                        .OrderBy(x => x.SortOrder)
                                        .ToList();
            var bankAccountsMinusCurrent = bankAccounts.Where(x => x.BankAccountId != bankAccountId).ToList();
            var bankAccount = bankAccounts.Single(x => x.BankAccountId == bankAccountId);

            var transactionTypes = new List<string> { TransactionTypes.Payment, TransactionTypes.Deposit };

            if (bankAccountsMinusCurrent.Count >= 1)
            {
                transactionTypes.Add(TransactionTypes.TransferTo);
                transactionTypes.Add(TransactionTypes.TransferFrom);
            }

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
                TransactionBalances = balances.OrderByDescending(x => x.transaction.TransactionDate).ThenByDescending(x => x.transaction.CreateDateUtc).ToList(),
                TransferToBankAccountsSelectList = new SelectList(bankAccountsMinusCurrent, "BankAccountId", "Name")
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

            if (!string.Equals(values["TransactionType"], TransactionTypes.TransferTo) &&
                !string.Equals(values["TransactionType"], TransactionTypes.TransferFrom) &&
                string.IsNullOrEmpty(values["Category"]))
            {
                throw new Exception("Category is required for payments or deposits.");
            }

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

            if (string.Equals(values["TransactionType"], TransactionTypes.TransferTo, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(values["TransactionType"], TransactionTypes.TransferFrom, StringComparison.OrdinalIgnoreCase))
            {
                PopulateTransferTransaction(bankAccount, bankAccountTransaction, values);
            }
            else
            {
                await PopulatePaymentOrDepositTransaction(bankAccountTransaction, values);
            }

            bankAccountTransaction.Payee = values["Payee"];
            bankAccountTransaction.TransactionType = values["TransactionType"];
            bankAccountTransaction.TransactionDate = DateTime.Parse(values["Date"]);
            bankAccountTransaction.TransactionEnteredDateUtc = DateTime.UtcNow;

            bankAccountTransactionRepo.PrepareEntityForCommit(bankAccountTransaction);

            await spendixDbContext.SaveChangesAsync();

            return RedirectToAction("Transactions", "Transaction", new { bankAccountId = bankAccount.BankAccountId });
        }

        private async Task PopulatePaymentOrDepositTransaction(BankAccountTransaction bankAccountTransaction, IFormCollection values)
        {
            var bankAccountTransactionCategory = await bankAccountTransactionCategoryRepo.FindByIdAsync(Guid.Parse(values["Category"]));
            bankAccountTransaction.BankAccountTransactionCategoryId = bankAccountTransactionCategory.BankAccountTransactionCategoryId;

            if (string.IsNullOrEmpty(values["SubCategory"]))
            {
                bankAccountTransaction.BankAccountTransactionSubCategoryId = null;
            }
            else
            {
                bankAccountTransaction.BankAccountTransactionSubCategoryId = Guid.Parse(values["SubCategory"]);
            }

            if (string.Equals(values["TransactionType"], TransactionTypes.Payment, StringComparison.OrdinalIgnoreCase))
            {
                bankAccountTransaction.Amount = decimal.Negate(decimal.Parse(values["Amount"]));
            }
            else
            {
                bankAccountTransaction.Amount = decimal.Parse(values["Amount"]);
            }
        }

        private void PopulateTransferTransaction(BankAccount bankAccount, BankAccountTransaction bankAccountTransaction, IFormCollection values)
        {
            if (string.Equals(values["TransactionType"], TransactionTypes.TransferTo))
            {
                bankAccountTransaction.Amount = decimal.Negate(decimal.Parse(values["Amount"]));
                bankAccountTransaction.TransferToBankAccountId = Guid.Parse(values["TransferBankAccountId"]);

                var transferToTransaction = new BankAccountTransaction
                {
                    BankAccountId = Guid.Parse(values["TransferBankAccountId"]),
                    TransactionType = TransactionTypes.TransferFrom,
                    TransactionDate = DateTime.Parse(values["Date"]),
                    Payee = values["Payee"],
                    Amount = decimal.Parse(values["Amount"]),
                    TransactionEnteredDateUtc = DateTime.UtcNow,
                    TransferFromBankAccountId = bankAccount.BankAccountId
                };

                bankAccountTransactionRepo.PrepareEntityForCommit(transferToTransaction);
            }
            else
            {
                bankAccountTransaction.Amount = decimal.Parse(values["Amount"]);
                bankAccountTransaction.TransferFromBankAccountId = Guid.Parse(values["TransferBankAccountId"]);

                var transferFromTransaction = new BankAccountTransaction
                {
                    BankAccountId = Guid.Parse(values["TransferBankAccountId"]),
                    TransactionType = TransactionTypes.TransferTo,
                    TransactionDate = DateTime.Parse(values["Date"]),
                    Payee = values["Payee"],
                    Amount = decimal.Parse(values["Amount"]),
                    TransactionEnteredDateUtc = DateTime.UtcNow,
                    TransferToBankAccountId = bankAccount.BankAccountId
                };

                bankAccountTransactionRepo.PrepareEntityForCommit(transferFromTransaction);
            }
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