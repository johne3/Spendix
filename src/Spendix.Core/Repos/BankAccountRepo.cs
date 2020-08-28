using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Accessors;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Spendix.Core.Repos
{
    public class BankAccountRepo : EntityRepo<BankAccount>
    {
        private readonly ILoggedInUserAccountAccessor loggedInUserAccountAccessor;

        public BankAccountRepo(SpendixDbContext spendixDbContext, ILoggedInUserAccountAccessor loggedInUserAccountAccessor) : base(spendixDbContext)
        {
            this.loggedInUserAccountAccessor = loggedInUserAccountAccessor;
        }

        public List<BankAccount> FindByLoggedInUserAccount()
        {
            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from ba in DataContext.BankAccounts
                    where ba.UserAccountId == userAccount.UserAccountId
                    select ba;

            return q.ToList();
        }

        public Task<List<BankAccount>> FindByLoggedInUserAccountAsync()
        {
            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from ba in DataContext.BankAccounts
                    where ba.UserAccountId == userAccount.UserAccountId
                    select ba;

            return q.ToListAsync();
        }

        public async Task<int> FindNextSortOrderAsync()
        {
            var userAccount = loggedInUserAccountAccessor.GetLoggedInUserAccount();

            var q = from ba in DataContext.BankAccounts
                    where ba.UserAccountId == userAccount.UserAccountId
                    select (int?)ba.SortOrder;

            var max = await q.MaxAsync() ?? 0;

            return max + 10;
        }
    }
}