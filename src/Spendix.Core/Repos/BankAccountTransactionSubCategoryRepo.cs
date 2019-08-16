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
    }
}