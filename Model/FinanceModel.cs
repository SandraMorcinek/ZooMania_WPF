using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooMania.Model
{
    public class FinanceModel
    {
        public int Id { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }

        public FinanceModel(int id, decimal revenue, decimal expenses, decimal profit)
        {
            Id = id;
            Revenue = revenue;
            Expenses = expenses;
            Profit = profit;
        }
    }
}
