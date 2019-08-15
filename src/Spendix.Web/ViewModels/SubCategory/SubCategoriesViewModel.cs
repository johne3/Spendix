using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spendix.Core.Entities;

namespace Spendix.Web.ViewModels.SubCategory
{
    public class SubCategoriesViewModel
    {
        public List<BankAccountTransactionCategory> PaymentCategories { get; set; }

        public List<BankAccountTransactionCategory> DepositCategories { get; set; }

        public List<BankAccountTransactionSubCategory> PaymentSubCategories { get; set; }

        public List<BankAccountTransactionSubCategory> DepositSubCategories { get; set; }
    }
}