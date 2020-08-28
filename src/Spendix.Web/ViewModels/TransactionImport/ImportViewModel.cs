using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Entities;

namespace Spendix.Web.ViewModels.TransactionImport
{
    public class ImportViewModel
    {
        public SelectList BankAccountSelectList { get; set; }

        public SelectList BankImportSourceSelectList { get; set; }

        public List<BankAccountTransactionCategory> PaymentTransactionCategories { get; set; }

        public List<BankAccountTransactionCategory> DepositTransactionCategories { get; set; }
    }
}