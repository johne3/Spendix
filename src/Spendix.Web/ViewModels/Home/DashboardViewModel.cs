using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Spendix.Core.Entities;

namespace Spendix.Web.ViewModels.Home
{
    public class DashboardViewModel
    {
        public SelectList BankAccountSelectList { get; set; }

        public SelectList RangeSelectList { get; set; }

        public List<(string Label, decimal Amount)> ChartData { get; set; }

        public decimal TotalIncome { get; set; }

        public decimal TotalExpenses { get; set; }
    }
}