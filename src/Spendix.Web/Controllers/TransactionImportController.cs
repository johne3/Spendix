using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spendix.Core.Repos;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Web.ViewModels.TransactionImport;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Constants;
using Microsoft.AspNetCore.Http;
using Spendix.Web.ResponseModels.TransactionImport;
using System.IO;
using System.Text.RegularExpressions;
using Spendix.Core.Accessors;
using Spendix.Core.Entities;
using Spendix.Core;

namespace Spendix.Web.Controllers
{
    [Route("Transactions")]
    [Authorize]
    public class TransactionImportController : BaseController
    {
        private readonly BankAccountRepo bankAccountRepo;
        private readonly BankAccountTransactionCategoryRepo bankAccountTransactionCategoryRepo;
        private readonly BankAccountTransactionRepo bankAccountTransactionRepo;

        private readonly SpendixDbContext spendixDbContext;

        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public TransactionImportController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            bankAccountRepo = serviceProvider.GetService<BankAccountRepo>();
            bankAccountTransactionCategoryRepo = serviceProvider.GetService<BankAccountTransactionCategoryRepo>();
            bankAccountTransactionRepo = serviceProvider.GetService<BankAccountTransactionRepo>();

            spendixDbContext = serviceProvider.GetService<SpendixDbContext>();

            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
        }

        [HttpGet, Route("Import")]
        public async Task<IActionResult> Import()
        {
            var bankAccounts = (await bankAccountRepo.FindByLoggedInUserAccountAsync())
                                        .OrderBy(x => x.SortOrder)
                                        .ToList();

            var supportedBankImportSources = BankImportSources.AllBankImportSources;

            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();
            var categories = await bankAccountTransactionCategoryRepo.FindByUserAccountAsync(userAccount);

            var paymentCategories = categories.Where(x => x.TransactionType == TransactionTypes.Payment)
                                              .OrderBy(x => x.Name)
                                              .ToList();

            var depositCategories = categories.Where(x => x.TransactionType == TransactionTypes.Deposit)
                                              .OrderBy(x => x.Name)
                                              .ToList();

            var vm = new ImportViewModel
            {
                BankAccounts = bankAccounts,
                BankAccountSelectList = new SelectList(bankAccounts, "BankAccountId", "Name"),
                BankImportSourceSelectList = new SelectList(supportedBankImportSources),
                PaymentTransactionCategories = paymentCategories,
                DepositTransactionCategories = depositCategories
            };

            return View(vm);
        }

        [HttpPost, Route("Import")]
        public async Task<IActionResult> Import(IFormCollection values)
        {
            var bankAccountId = Guid.Parse(values["BankAccountId"]);
            var bankAccount = await bankAccountRepo.FindByIdAsync(bankAccountId);
            var transactionNumbers = GetNumbersFromFormKeys("TransactionDateTime_", values);

            foreach (var number in transactionNumbers)
            {
                if (string.IsNullOrEmpty(values[$"Save_{number}"]))
                {
                    continue;
                }

                var transactionEnteredDateUtc = DateTime.UtcNow;

                var transaction = new BankAccountTransaction
                {
                    BankAccountId = bankAccountId,
                    TransactionType = values[$"TransactionType_{number}"],
                    TransactionDateTime = DateTime.Parse(values[$"TransactionDateTime_{number}"]),
                    TransactionEnteredDateUtc = transactionEnteredDateUtc,
                    Payee = values[$"Payee_{number}"],
                    Amount = decimal.Parse(values[$"Amount_{number}"])
                };

                var category = values[$"CategoryId_{number}"].ToString();

                if (category.StartsWith("TransferTo_"))
                {
                    var transferToBankAccountId = Guid.Parse(category.Replace("TransferTo_", ""));

                    var transferTransaction = new BankAccountTransaction
                    {
                        BankAccountId = transferToBankAccountId,
                        TransactionType = TransactionTypes.TransferTo,
                        TransactionDateTime = DateTime.Parse(values[$"TransactionDateTime_{number}"]),
                        TransactionEnteredDateUtc = transactionEnteredDateUtc,
                        Payee = values[$"Payee_{number}"],
                        Amount = decimal.Negate(decimal.Parse(values[$"Amount_{number}"])),
                        TransferFromBankAccountId = bankAccountId,
                        TransferFromBankAccount = bankAccount
                    };

                    transaction.Payee = values[$"Payee_{number}"];
                    transaction.TransactionType = TransactionTypes.TransferFrom;
                    transaction.TransferToBankAccountId = transferToBankAccountId;

                    bankAccountTransactionRepo.PrepareEntityForCommit(transferTransaction);
                }
                else if (category.StartsWith("TransferFrom_"))
                {
                    var transferFromBankAccountId = Guid.Parse(category.Replace("TransferFrom_", ""));

                    var transferTransaction = new BankAccountTransaction
                    {
                        BankAccountId = transferFromBankAccountId,
                        TransactionType = TransactionTypes.TransferFrom,
                        TransactionDateTime = DateTime.Parse(values[$"TransactionDateTime_{number}"]),
                        TransactionEnteredDateUtc = transactionEnteredDateUtc,
                        Payee = values[$"Payee_{number}"],
                        Amount = decimal.Negate(decimal.Parse(values[$"Amount_{number}"])),
                        TransferFromBankAccountId = bankAccountId,
                        TransferToBankAccount = bankAccount
                    };

                    transaction.Payee = values[$"Payee_{number}"];
                    transaction.TransactionType = TransactionTypes.TransferTo;
                    transaction.TransferFromBankAccountId = transferFromBankAccountId;

                    bankAccountTransactionRepo.PrepareEntityForCommit(transferTransaction);
                }
                else
                {
                    transaction.BankAccountTransactionCategoryId = Guid.Parse(values[$"CategoryId_{number}"]);

                    if (!string.IsNullOrEmpty(values[$"SubCategoryId_{number}"]))
                    {
                        transaction.BankAccountTransactionSubCategoryId = Guid.Parse(values[$"SubCategoryId_{number}"]);
                    }
                }

                bankAccountTransactionRepo.PrepareEntityForCommit(transaction);
            }

            await spendixDbContext.SaveChangesAsync();

            return SetAlertMessageAndRedirect("Dashboard", "Home", "Transaction successfully imported!", Models.AlertMessageType.Success);
        }

        private List<int> GetNumbersFromFormKeys(string prefix, IFormCollection values)
        {
            var numberRegex = new Regex($"{prefix}(?<Number>\\d*)", RegexOptions.CultureInvariant | RegexOptions.Compiled);

            var numbers = new List<int>();

            foreach (var key in values.Keys)
            {
                var match = numberRegex.Match(key.ToString());

                if (match.Success)
                {
                    numbers.Add(int.Parse(match.Groups["Number"].Value));
                }
            }

            return numbers;
        }

        [HttpPost, Route("ProcessImport")]
        public async Task<IActionResult> ProcessImport(IFormCollection values)
        {
            var bankAccountId = Guid.Parse(values["BankAccountId"]);
            var bankAccount = await bankAccountRepo.FindByIdAsync(bankAccountId);

            var bankImportSource = values["BankImportSource"];
            var file = values.Files.First();

            List<ProcessImportResponseModel> transactions = null;

            if (string.Equals(bankImportSource, BankImportSources.AllyBank))
            {
                transactions = await BuildResponseFromAllyBankImport(file);
            }

            return Ok(new
            {
                success = true,
                bankAccountId,
                bankAccountName = bankAccount.Name,
                transactions
            });
        }

        private async Task<List<ProcessImportResponseModel>> BuildResponseFromAllyBankImport(IFormFile file)
        {
            var models = new List<ProcessImportResponseModel>();
            var index = 0;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var line = await reader.ReadLineAsync();

                    if (index == 0)
                    {
                        //Skip header row
                        index++;
                        continue;
                    }

                    var parser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    var items = parser.Split(line);
                    var type = items[3];

                    var transactionType = TransactionTypes.Payment;

                    if (string.Equals(type, "Deposit"))
                    {
                        transactionType = TransactionTypes.Deposit;
                    }

                    var transactionDate = DateTime.Parse(items[0]);
                    transactionDate = transactionDate.Date + TimeSpan.Parse(items[1]);

                    models.Add(new ProcessImportResponseModel
                    {
                        TransactionDateTime = transactionDate.ToString(),
                        DisplayDate = transactionDate.ToShortDateString(),
                        Payee = items[4].Replace("\"", ""),
                        Amount = items[2],
                        TransactionType = transactionType,
                        IsPaymentTransactionType = string.Equals(transactionType, TransactionTypes.Payment)
                    });

                    index++;
                }
            }

            return models;
        }
    }
}