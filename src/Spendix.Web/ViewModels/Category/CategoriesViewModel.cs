using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spendix.Core.Entities;

namespace Spendix.Web.ViewModels.Category
{
    public class CategoriesViewModel
    {
        public List<BankAccountTransactionCategory> PaymentCategories { get; set; }

        public List<BankAccountTransactionCategory> DepositCategories { get; set; }
    }
}