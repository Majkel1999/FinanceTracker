using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Diagnostics;
/* Api Key AlphaVantage
* 8PXA60GU992EV4KA 
*/
/* Api Key financialModelingPrep
 * b9d32703beec60431f239bcfae715d7f
 */
namespace FinanceTracker
{
    /// <summary>
    /// Static class responsible for obtaining data from the server by public API.
    /// APIKey can be obtained at https://financialmodelingprep.com/
    /// </summary>
    public static class ApiRequest
    {
        private const string APIKey = "0959a6645818c0ead96715ab44135e89";
        private static HttpClient m_client = new HttpClient();
        /// <summary>
        /// Creates static class with the base URL of the default website.
        /// Prepare for accepting JSON
        /// </summary>
        static ApiRequest()
        {
            m_client.BaseAddress = new Uri("https://financialmodelingprep.com/");
            m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        /// <summary>
        /// Gets StockPrice from the server for the given index
        /// </summary>
        /// <param name="stockIndex">Index symbol for which to get data for</param>
        /// <returns>Data obtained from the server</returns>
        public static StockPrice GetStockPrice(string stockIndex)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            HttpResponseMessage response = m_client.GetAsync("/api/v3/quote-short/" + stockIndex + "?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            result = result.TrimStart('[');
            result = result.TrimEnd(']');
            Trace.Write(result);
            return JsonConvert.DeserializeObject<StockPrice>(result);

        }
        /// <summary>
        /// Gets a list of all stock indexes avalible on the market. Removes all the indexes
        /// for which the exchange is not primary by filtering symbols for '.', price is 0 or 
        /// name is null or empty.
        /// </summary>
        /// <returns>List of all stock indexes with full data</returns>
        public static List<StockIndex> GetStockIndexes()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            HttpResponseMessage response = m_client.GetAsync("/api/v3/available-traded/list?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            List<StockIndex> indexList = JsonConvert.DeserializeObject<StockIndex[]>(result).ToList();
            indexList.RemoveAll(s => s.symbol.Contains('.'));
            indexList.RemoveAll(s => s.price == 0);
            indexList.RemoveAll(s => s.name == null);
            indexList.RemoveAll(s => s.name == "");
            return indexList;

        }
        /// <summary>
        /// Gets a list of all historical price data for a given index.
        /// </summary>
        /// <param name="stockIndex">Index symbol for which to get data for</param>
        /// <returns>List of all historical prices</returns>
        public static List<HistoricalIndexData> GetHistoricalData(string stockIndex)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            HttpResponseMessage response = m_client.GetAsync("/api/v3/historical-price-full/" + stockIndex + "?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
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
        /// /// Gets a list of historical price data for a given index from the given start date.
        /// </summary>
        /// <param name="stockIndex">Index symbol for which to get data for</param>
        /// <param name="startDate">Date from which to get data from</param>
        /// <returns>List of historical prices</returns>
        public static List<HistoricalIndexData> GetHistoricalData(string stockIndex, DateTime startDate)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["from"] = startDate.ToString("yyyy-MM-dd");
            query["to"] = DateTime.Today.ToString("yyyy-MM-dd");
            query["apikey"] = APIKey;
            HttpResponseMessage response = m_client.GetAsync("/api/v3/historical-price-full/" + stockIndex + "?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
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
