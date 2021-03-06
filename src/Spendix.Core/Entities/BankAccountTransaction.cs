﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Entities
{
    public class BankAccountTransaction : Entity
    {
        public Guid BankAccountTransactionId { get; set; }

        public Guid BankAccountId { get; set; }

        public Guid? BankAccountTransactionCategoryId { get; set; }

        public Guid? BankAccountTransactionSubCategoryId { get; set; }

        public string TransactionType { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public DateTime TransactionEnteredDateUtc { get; set; }

        public string Payee { get; set; }

        public decimal Amount { get; set; }

        public Guid? TransferToBankAccountId { get; set; }

        public Guid? TransferFromBankAccountId { get; set; }

        public BankAccountTransactionCategory BankAccountTransactionCategory { get; set; }

        public BankAccountTransactionSubCategory BankAccountTransactionSubCategory { get; set; }

        public BankAccount TransferToBankAccount { get; set; }

        public BankAccount TransferFromBankAccount { get; set; }
    }
}