using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spendix.Core.Accessors;
using Microsoft.Extensions.DependencyInjection;
using Spendix.Core.Constants;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionRepo : EntityRepo<BankAccountTransaction>
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public BankAccountTransactionRepo(IServiceProvider serviceProvider, SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
            loggedInUserAccountAccessor = serviceProvider.GetService<ILoggedInUserAccountAccessor>();
        }

        public Task<List<BankAccountTransaction>> FindByBankAccountAsync(BankAccount bankAccount)
        {
            var q = from bat in DataContext.BankAccountTransactions
                                                .Include(x => x.BankAccountTransactionCategory)
                                                .Include(x => x.BankAccountTransactionSubCategory)
                    where bat.BankAccountId == bankAccount.BankAccountId
                    select bat;

            return q.ToListAsync();
        }

        public async Task<List<(int Month, int Year)>> FindTransactionMonthsAndYearsByLoggedInUserAccount()
        {
            var loggedInUserAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from bat in DataContext.BankAccountTransactions
                    join ba in DataContext.BankAccounts on bat.BankAccountId equals ba.BankAccountId
                    where ba.UserAccountId == loggedInUserAccount.UserAccountId
                    group bat by new { bat.TransactionDate.Month, bat.TransactionDate.Year } into g
                    select new
                    {
                        g.Key.Month,
                        g.Key.Year
                    };

            var results = await q.ToListAsync();

            return results.Select(x => (x.Month, x.Year)).ToList();
        }

        public Task<decimal> FindTotalIncomeByDateRange(DateTime startDate, DateTime endDate, BankAccount bankAccount)
        {
            var loggedInUserAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from bat in DataContext.BankAccountTransactions
                    join ba in DataContext.BankAccounts on bat.BankAccountId equals ba.BankAccountId
                    join batc in DataContext.BankAccountTransactionCategories on bat.BankAccountTransactionCategoryId equals batc.BankAccountTransactionCategoryId
                    where ba.UserAccountId == loggedInUserAccount.UserAccountId
                    && batc.TransactionType == TransactionTypes.Deposit
                    && bat.TransactionDate >= startDate
                    && bat.TransactionDate <= endDate
                    select bat;

            if (bankAccount != null)
            {
                q = q.Where(x => x.BankAccountId == bankAccount.BankAccountId);
            }

            return q.Select(x => x.Amount).SumAsync();
        }

        public Task<decimal> FindTotalExpensesByDateRange(DateTime startDate, DateTime endDate, BankAccount bankAccount)
        {
            var loggedInUserAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from bat in DataContext.BankAccountTransactions
                    join ba in DataContext.BankAccounts on bat.BankAccountId equals ba.BankAccountId
                    join batc in DataContext.BankAccountTransactionCategories on bat.BankAccountTransactionCategoryId equals batc.BankAccountTransactionCategoryId
                    where ba.UserAccountId == loggedInUserAccount.UserAccountId
                    && batc.TransactionType == TransactionTypes.Payment
                    && bat.TransactionDate >= startDate
                    && bat.TransactionDate <= endDate
                    select bat;

            if (bankAccount != null)
            {
                q = q.Where(x => x.BankAccountId == bankAccount.BankAccountId);
            }

            return q.Select(x => x.Amount).SumAsync();
        }

        public async Task<List<(string CategoryName, decimal Amount)>> FindCategoryTotalsByDateRange(DateTime startDate, DateTime endDate, BankAccount bankAccount)
        {
            var loggedInUserAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from bat in DataContext.BankAccountTransactions
                    join ba in DataContext.BankAccounts on bat.BankAccountId equals ba.BankAccountId
                    join batc in DataContext.BankAccountTransactionCategories on bat.BankAccountTransactionCategoryId equals batc.BankAccountTransactionCategoryId
                    where ba.UserAccountId == loggedInUserAccount.UserAccountId
                    && batc.TransactionType == TransactionTypes.Payment
                    && batc.IncludeInStatistics == true
                    && bat.TransactionDate >= startDate
                    && bat.TransactionDate <= endDate
                    select new
                    {
                        bat,
                        batc
                    };

            if (bankAccount != null)
            {
                q = q.Where(x => x.bat.BankAccountId == bankAccount.BankAccountId);
            }

            var categoryAmounts = await (from entities in q
                                         group entities by entities.batc.BankAccountTransactionCategoryId into g
                                         select new
                                         {
                                             CategoryName = g.First().batc.Name,
                                             Amount = g.Sum(x => x.bat.Amount)
                                         }).ToListAsync();

            return categoryAmounts.Select(x => (x.CategoryName, x.Amount)).ToList();
        }
    }
}