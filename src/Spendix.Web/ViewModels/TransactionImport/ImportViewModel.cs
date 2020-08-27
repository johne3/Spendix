using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Spendix.Web.ViewModels.TransactionImport
{
    public class ImportViewModel
    {
        public SelectList BankAccountSelectList { get; set; }

        public SelectList BankImportSourceSelectList { get; set; }
    }
}