using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Constants
{
    public static class BankImportSources
    {
        public static string AllyBank => "Ally Bank";

        public static List<string> AllBankImportSources => new List<string>
        {
            AllyBank
        };
    }
}