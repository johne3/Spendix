using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class BankAccountTransactionSubCategory : Entity
    {
        public Guid BankAccountTransactionSubCategoryId { get; set; }

        public Guid BankAccountTransactionCategoryId { get; set; }

        public string Name { get; set; }
    }
}
