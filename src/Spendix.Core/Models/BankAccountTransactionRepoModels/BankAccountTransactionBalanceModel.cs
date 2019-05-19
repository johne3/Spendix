using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Models.BankAccountTransactionRepoModels
{
    public class BankAccountTransactionBalanceModel
    {
        public Guid BankAccountTransactionId { get; set; }

        public decimal? CurrentBalance { get; set; }
    }
}