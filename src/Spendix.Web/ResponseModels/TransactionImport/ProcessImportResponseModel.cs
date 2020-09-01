using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spendix.Web.ResponseModels.TransactionImport
{
    public class ProcessImportResponseModel
    {
        public string Date { get; set; }

        public string Payee { get; set; }

        public string Amount { get; set; }

        public string TransactionType { get; set; }

        public bool IsPaymentTransactionType { get; set; }
    }
}