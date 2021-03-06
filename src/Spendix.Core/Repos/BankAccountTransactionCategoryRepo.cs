﻿using System;
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

        public Task<BankAccountTransactionCategory> FindByIdWithIncludesAsync(Guid categoryId)
        {
            var q = from batc in DataContext.BankAccountTransactionCategories.Include(x => x.BankAccountTransactionSubCategories)
                    where batc.BankAccountTransactionCategoryId == categoryId
                    select batc;

            return q.SingleAsync();
        }

        public Task<List<BankAccountTransactionCategory>> FindByUserAccountAsync(UserAccount userAccount)
        {
            var q = from batc in DataContext.BankAccountTransactionCategories.Include(x => x.BankAccountTransactionSubCategories)
                    where batc.UserAccountId == userAccount.UserAccountId
                    select batc;

            return q.ToListAsync();
        }

        public Task<List<BankAccountTransactionCategory>> FindByUserAccountWithIncludesAsync(UserAccount userAccount)
        {
            var q = from batc in DataContext.BankAccountTransactionCategories.Include(x => x.BankAccountTransactionSubCategories)
                    where batc.UserAccountId == userAccount.UserAccountId
                    select batc;

            return q.ToListAsync();
        }

        public Task<List<BankAccountTransactionCategory>> FindByUserAccountAndTransactionType(UserAccount userAccount, string transactionType)
        {
            var q = from batc in DataContext.BankAccountTransactionCategories
                    where batc.UserAccountId == userAccount.UserAccountId
                    && batc.TransactionType == transactionType
                    select batc;

            return q.ToListAsync();
        }
    }
}