using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Spendix.Core.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionSubCategoryRepo : EntityRepo<BankAccountTransactionSubCategory>
    {
        public BankAccountTransactionSubCategoryRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
        }

        public Task<List<BankAccountTransactionSubCategory>> FindByUserAccountWithIncludesAsync(UserAccount userAccount)
        {
            var q = from batsc in DataContext.BankAccountTransactionSubCategories.Include(x => x.BankAccountTransactionCategory)
                    join batc in DataContext.BankAccountTransactionCategories on batsc.BankAccountTransactionCategoryId equals batc.BankAccountTransactionCategoryId
                    where batc.UserAccountId == userAccount.UserAccountId
                    select batsc;

            return q.ToListAsync();
        }
    }
}