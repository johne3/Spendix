using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionRepo : EntityRepo<BankAccountTransaction>
    {
        public BankAccountTransactionRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
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
    }
}