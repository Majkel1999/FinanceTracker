using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinanceTracker
{
    /// <summary>
    ///    Klasa statyczna odpowiedzialna za otrzymywanie i parsowanie danych z serwera
    /// </summary>
    public static class FinancialData
    {
        /// <summary>
        /// Metoda pobiera dane z serwera i deserializuje je do obiektów.
        /// Pobrane dane są deserializowane z JSON.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego pobrac cene</param>
        /// <returns>Tablica StockPrice[] o jednym elemencie</returns>
        public async static Task<StockPrice[]> GetStockIndexPrice(string stockIndex)
        {
            string apiTask = await ApiRequest.GetStockPrice(stockIndex);
            return JsonConvert.DeserializeObject<StockPrice[]>(apiTask);
        }
        /// <summary>
        /// Metoda pobiera wszystkie indeksy dostepne na gieldzie.
        /// Usuwane sa wszystkie indeksy, ktorych nazwa zawiera znak '.', cena jest rowna 0 
        /// lub nazwa jest pusta, lub nie ustawiona.
        /// </summary>
        /// <returns>Lista StockIndex na ktorej sa wszystkie dostepne symbole</returns>
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
        /// <summary>
        /// Metoda pobiera pełne dane historyczne i deserializuje je do obiektu.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego pobrac dane</param>
        /// <returns>Lista pelnych danych historycznych</returns>
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
        /// <summary>
        /// Metoda pobiera dane historyczne i deserializuje je do obiektu.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego pobrac dane</param>
        /// <param name="startDate">Data od ktorej pobrac dane</param>
        /// <returns>Lista danych historycznych od zadanej daty</returns>
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
