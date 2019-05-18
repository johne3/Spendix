using Microsoft.EntityFrameworkCore;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Spendix.Core.Repos
{
    public class UserAccountRepo : EntityRepo<UserAccount>
    {
        public UserAccountRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
        }

        public async Task<bool> FindIsUniqueEmailAddress(string emailAddress)
        {
            var q = FindByEmailAddressQuery(emailAddress);

            return await q.AnyAsync() == false;
        }

        public Task<UserAccount> FindByEmailAddress(string emailAddress)
        {
            var q = FindByEmailAddressQuery(emailAddress);

            return q.SingleOrDefaultAsync();
        }

        private IQueryable<UserAccount> FindByEmailAddressQuery(string emailAddress)
        {
            var q = from ua in DataContext.UserAccounts
                    where ua.EmailAddress == emailAddress
                    select ua;

            return q;
        }
    }
}
