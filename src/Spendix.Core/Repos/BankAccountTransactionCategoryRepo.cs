using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Spendix.Core.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionCategoryRepo : EntityRepo<BankAccountTransactionCategory>
    {
        public BankAccountTransactionCategoryRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
        }

        public Task<List<BankAccountTransactionCategory>> FindByTransactionType(UserAccount userAccount, string transactionType)
        {
            var q = from batc in DataContext.BankAccountTransactionCategories
                    where batc.UserAccountId == userAccount.UserAccountId
                    && batc.TransactionType == transactionType
                    select batc;

            return q.ToListAsync();
        }
    }
}