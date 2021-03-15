using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker
{
    public class StockIndex
    {
        public string symbol;
        public string name;
        public double price;
    }
    public class HistoricalIndexData
    {
        public string symbol;
        public DateTime date;
        public double price;
        public double volume;
        public double changeOverTime;
    }
}
