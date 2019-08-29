using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Spendix.Core.Accessors;
using Microsoft.Extensions.DependencyInjection;

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

            return results.Select(x => (x.Month, x.Year))
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
        }
    }
}