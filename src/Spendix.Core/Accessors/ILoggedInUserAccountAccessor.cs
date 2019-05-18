using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;

namespace Spendix.Core.Accessors
{
    public interface ILoggedInUserAccountAccessor
    {
        UserAccount GetLoggedInUserAccount();

        Guid GetLoggedInUserAccountId();
    }
}