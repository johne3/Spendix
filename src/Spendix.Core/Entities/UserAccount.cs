using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class UserAccount : Entity
    {
        public Guid UserAccountId { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
