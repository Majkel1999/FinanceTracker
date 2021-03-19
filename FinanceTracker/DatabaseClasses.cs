using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker
{
    public class StockIndex
    {
        [Key] public string symbol { get; set; }
        public string name { get; set; }
        public double price { get; set; }
    }
    public class HistoricalIndexData
    {
        [Key] public int ID { get; set; }
        public string symbol { get; set; }
        public DateTime date { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public double changeOverTime { get; set; }
    }

    public class MyStock
    {
        [Key] public int transactionID { get; set; }
        public string symbol { get; set; }
        public DateTime transactionDate { get; set; }
        public double transactionVolume { get; set; }
        public double indexPrice { get; set; }
        public double profit { get; set; }

    }

}
