using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Constants
{
    public static class TransactionTypes
    {
        public static string Payment => "Payment";

        public static string Deposit => "Deposit";

        public static string TransferTo => "Transfer To";

        public static string TransferFrom => "Transfer From";
    }
}