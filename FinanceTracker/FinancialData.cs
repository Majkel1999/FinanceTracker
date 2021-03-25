using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinanceTracker
{
    public static class FinancialData
    {
        public async static Task<StockPrice[]> GetStockIndexPrice(string stockIndex)
        {
            string apiTask = await ApiRequest.GetStockPrice(stockIndex);
            return JsonConvert.DeserializeObject<StockPrice[]>(apiTask);
        }
        public async static Task<List<StockIndex>> GetAllStockIndexes()
        {
            string apiTask = await ApiRequest.GetStockIndexes();
            List<StockIndex> indexList = JsonConvert.DeserializeObject<StockIndex[]>(apiTask).ToList();
            indexList.RemoveAll(s => s.symbol.Contains('.'));
            indexList.RemoveAll(s => s.price == 0);
            indexList.RemoveAll(s => s.name == null);
            indexList.RemoveAll(s => s.name == "");
            return indexList;
        }
        public async static Task<List<HistoricalIndexData>> GetHistoricalIndexData(string stockIndex)
        {
            string result = await ApiRequest.GetHistoricalData(stockIndex);
            HistoricalDataList historicalData = JsonConvert.DeserializeObject<HistoricalDataList>(result);
            List<HistoricalIndexData> historicalIndexData = new List<HistoricalIndexData>();
            foreach (HistoricalData data in historicalData.historical)
            {
                historicalIndexData.Add(new HistoricalIndexData
                {
                    symbol = historicalData.symbol,
                    price = data.high,
                    date = data.date,
                    changeOverTime = data.changeOverTime,
                    volume = data.volume
                });
            }
            return historicalIndexData;
        }
        public async static Task<List<HistoricalIndexData>> GetHistoricalIndexData(string stockIndex,DateTime startDate)
        {
            string result = await ApiRequest.GetHistoricalData(stockIndex,startDate);
            HistoricalDataList historicalData = JsonConvert.DeserializeObject<HistoricalDataList>(result);
            List<HistoricalIndexData> historicalIndexData = new List<HistoricalIndexData>();
            foreach (HistoricalData data in historicalData.historical)
            {
                historicalIndexData.Add(new HistoricalIndexData
                {
                    symbol = historicalData.symbol,
                    price = data.high,
                    date = data.date,
                    changeOverTime = data.changeOverTime,
                    volume = data.volume
                });
            }
            return historicalIndexData;
        }
    }
}
