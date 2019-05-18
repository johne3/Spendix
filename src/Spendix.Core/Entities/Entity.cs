using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public abstract class Entity
    {
        public bool IsDeleted { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public Guid CreateUserAccountId { get; set; }

        public DateTime ModifyDateUtc { get; set; }

        public Guid ModifyUserAccountId { get; set; }
    }
}
