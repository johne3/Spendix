using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class BankAccountTransactionCategory : Entity
    {
        public Guid BankAccountTransactionCategoryId { get; set; }

        public Guid UserAccountId { get; set; }

        public string TransactionType { get; set; }

        public string Name { get; set; }
    }
}