using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker
{
    public class StockPrice
    {
        public string symbol;
        public double price;
        public ulong volume;

    }
    public class HistoricalData
    {
        public DateTime date;
        public double high;
        public double volume;
        public double changeOverTime;
    }

    public class HistoricalDataList
    {
        public string symbol;
        public List<HistoricalData> historical =new List<HistoricalData>();
    }
}
