using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionSubCategoryRepo : EntityRepo<BankAccountTransactionSubCategory>
    {
        public BankAccountTransactionSubCategoryRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
        }
    }
}
