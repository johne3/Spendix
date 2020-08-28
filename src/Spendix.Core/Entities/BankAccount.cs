using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class BankAccount : Entity
    {
        public Guid BankAccountId { get; set; }

        public Guid UserAccountId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public decimal OpeningBalance { get; set; }

        public int SortOrder { get; set; }
    }
}