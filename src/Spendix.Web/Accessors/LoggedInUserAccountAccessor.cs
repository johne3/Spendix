using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Spendix.Core.Accessors;
using Spendix.Core.Entities;
using Spendix.Core.Repos;

namespace Spendix.Web.Accessors
{
    public class LoggedInUserAccountAccessor : ILoggedInUserAccountAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserAccountRepo userAccountRepo;
        private UserAccount loggedInUserAccount;

        public LoggedInUserAccountAccessor(IHttpContextAccessor contextAccessor, UserAccountRepo userAccountRepo)
        {
            httpContextAccessor = contextAccessor;
            this.userAccountRepo = userAccountRepo;
        }

        public UserAccount GetLoggedInUserAccount()
        {
            if (loggedInUserAccount == null)
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    var idClaim = httpContext.User.Claims.Single(x => x.Type == "UserAccountId");
                    var userAccountId = Guid.Parse(idClaim.Value);

                    //TODO: Get this from cache
                    var userAccount = userAccountRepo.FindById(userAccountId);

                    loggedInUserAccount = userAccount;
                }
            }

            return loggedInUserAccount;
        }
    }
}