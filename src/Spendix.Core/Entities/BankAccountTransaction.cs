using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class BankAccountTransaction : Entity
    {
        public Guid BankAccountTransactionId { get; set; }

        public Guid BankAccountId { get; set; }

        public Guid BankAccountTransactionCategoryId { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime TransactionEnteredDateUtc { get; set; }

        public string Payee { get; set; }

        public decimal Amount { get; set; }

        public BankAccountTransactionCategory BankAccountTransactionCategory { get; set; }
    }
}