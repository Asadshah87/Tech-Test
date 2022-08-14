using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Model
{
    public class OrderMonthlyProfit
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal? Profit { get; set; }
        public int Year { get; set; }
    }
}
