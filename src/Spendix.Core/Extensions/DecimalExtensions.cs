using System;
using System.Collections.Generic;
using System.Text;

namespace Spendix.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatCurrency(this decimal dec)
        {
            return string.Format("${0:#.00}", dec);
        }
    }
}