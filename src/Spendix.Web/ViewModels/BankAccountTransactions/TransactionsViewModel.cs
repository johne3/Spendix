using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Entities;

namespace Spendix.Web.ViewModels.BankAccountTransactions
{
    public class TransactionsViewModel
    {
        public BankAccountTransaction SelectedBankAccountTransaction { get; set; }

        public SelectList TransactionTypeSelectList { get; set; }

        public Spendix.Core.Entities.BankAccount BankAccount { get; set; }

        public List<BankAccountTransaction> Transactions { get; set; }

        public List<(BankAccountTransaction transaction, decimal balance)> TransactionBalances { get; set; }

        public SelectList TransferToBankAccountsSelectList { get; set; }
    }
}