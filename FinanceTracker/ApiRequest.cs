using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
/* Api Key AlphaVantage
* 8PXA60GU992EV4KA 
*/
/* Api Key financialModelingPrep
 * b9d32703beec60431f239bcfae715d7f
 * 0959a6645818c0ead96715ab44135e89
 */
namespace FinanceTracker
{
    /// <summary>
    /// Klasa statyczna odpowiedzialna za obsługę komunikacji z API.
    /// Klucz APIKey można dostać na  https://financialmodelingprep.com/
    /// </summary>
    public static class ApiRequest
    {
        /// <summary>
        /// Zmienna używana do testowania klasy.
        /// W trybie testowym zwraca dane dla AAPL.
        /// </summary>
        public static bool Debug = false;
        /// <summary>
        /// Klucz api potrzebny do zapytan do serwera.
        /// </summary>
        private const string APIKey = "0959a6645818c0ead96715ab44135e89";
        private static HttpClient m_client = new HttpClient();

        static ApiRequest()
        {
            m_client.BaseAddress = new Uri("https://financialmodelingprep.com/");
            m_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        /// <summary>
        /// Metoda zwraca otrzymany string z aktualna cena danego symbolu.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego chcemy otrzymac cene</param>
        /// <returns>Zawartosc wiadomosci z serwera</returns>
        public async static Task<string> GetStockPrice(string stockIndex)
        {
            if (Debug)
            {
                return "[ {\"symbol\" : \"AAPL\",  \"price\" : 120.58000000,  \"volume\" : 73077351} ]";
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            Task<HttpResponseMessage> responseTask = m_client.GetAsync("/api/v3/quote-short/" + stockIndex + "?" + query.ToString());
            await responseTask;
            return await responseTask.Result.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// Pobiera pelna liste wszystkich dostepnych symboli na gieldzie.
        /// </summary>
        /// <returns>Lista wszystkich dostepnych symboli na gieldzie</returns>
        public async static Task<string> GetStockIndexes()
        {
            if (Debug)
            {
                return @"[ {
                        ""symbol"" : ""AAPL"",
                        ""name"" : ""Apple Inc."",
                        ""price"" : 0.58,
                        ""exchange"" : ""Nasdaq Global Select""},
                        {
                        ""symbol"" : ""CMCSA"",
                        ""name"" : ""Comcast Corp"",
                        ""price"" : 52.75,
                        ""exchange"" : ""Nasdaq Global Select"" },
                        {
                        ""symbol"" : ""AAPL.TEST"",
                        ""name"" : ""DOTTEST"",
                        ""price"" : 120.58,
                        ""exchange"" : ""Nasdaq Global Select""},
                        {
                        ""symbol"" : ""PRICETEST"",
                        ""name"" : ""PRICETEST"",
                        ""price"" : 0,
                        ""exchange"" : ""Nasdaq Global Select""},
                        {
                        ""symbol"" : ""NULLTEST"",
                        ""name"" : """",
                        ""price"" : 120.58,
                        ""exchange"" : ""Nasdaq Global Select""} ] ";
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            Task<HttpResponseMessage> responseTask = m_client.GetAsync("/api/v3/available-traded/list?" + query.ToString());
            await responseTask;
            return await responseTask.Result.Content.ReadAsStringAsync();

        }
        /// <summary>
        /// Metoda pobierajaca pelne dane historyczne dla danego symbolu z zewnetrznego API.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego pobrac dane</param>
        /// <returns>Pelna lista danych historycznych</returns>
        public async static Task<string> GetHistoricalData(string stockIndex)
        {
            if (Debug)
            {
                return @"{
                      ""symbol"" : ""AAPL"",
                      ""historical"" : [ {
                        ""date"" : ""2021-03-24"",
                        ""open"" : 122.82,
                        ""high"" : 122.9,
                        ""low"" : 120.065,
                        ""close"" : 120.09,
                        ""adjClose"" : 120.09,
                        ""volume"" : 8.92177E7,
                        ""unadjustedVolume"" : 8.92177E7,
                        ""change"" : -2.73,
                        ""changePercent"" : -2.223,
                        ""vwap"" : 121.01833,
                        ""label"" : ""March 24, 21"",
                        ""changeOverTime"" : -0.02223
                      }, {
                        ""date"" : ""2021-03-23"",
                        ""open"" : 123.330002,
                        ""high"" : 124.239998,
                        ""low"" : 122.139999,
                        ""close"" : 122.540001,
                        ""adjClose"" : 122.540001,
                        ""volume"" : 9.51957E7,
                        ""unadjustedVolume"" : 9.51957E7,
                        ""change"" : -0.79,
                        ""changePercent"" : -0.641,
                        ""vwap"" : 122.97333,
                        ""label"" : ""March 23, 21"",
                        ""changeOverTime"" : -0.00641
                      }, {
                        ""date"" : ""2021-03-22"",
                        ""open"" : 120.330002,
                        ""high"" : 123.870003,
                        ""low"" : 120.260002,
                        ""close"" : 123.389999,
                        ""adjClose"" : 123.389999,
                        ""volume"" : 1.119123E8,
                        ""unadjustedVolume"" : 1.119123E8,
                        ""change"" : 3.06,
                        ""changePercent"" : 2.543,
                        ""vwap"" : 122.50667,
                        ""label"" : ""March 22, 21"",
                        ""changeOverTime"" : 0.02543
                      }, {
                        ""date"" : ""2021-03-19"",
                        ""open"" : 119.900002,
                        ""high"" : 121.43,
                        ""low"" : 119.68,
                        ""close"" : 119.989998,
                        ""adjClose"" : 119.989998,
                        ""volume"" : 1.855495E8,
                        ""unadjustedVolume"" : 1.855495E8,
                        ""change"" : 0.09,
                        ""changePercent"" : 0.075,
                        ""vwap"" : 120.36667,
                        ""label"" : ""March 19, 21"",
                        ""changeOverTime"" : 7.5E-4
                      } ]
                    }";
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            var responseTask = m_client.GetAsync("/api/v3/historical-price-full/" + stockIndex + "?" + query.ToString());
            await responseTask;
            Trace.WriteLine("Response:" + responseTask.Result.StatusCode);
            return await responseTask.Result.Content.ReadAsStringAsync();

        }
        /// <summary>
        /// Metoda pobierajaca dane historyczne dla danego symbolu z zewnetrznego API,
        /// rozpoczynajac od zadanej daty.
        /// </summary>
        /// <param name="stockIndex">Symbol dla ktorego pobrac dane</param>
        /// <param name="startDate">Data od ktorej rozpoczac dane</param>
        /// <returns>Lista danych historycznych</returns>
        public async static Task<string> GetHistoricalData(string stockIndex, DateTime startDate)
        {
            if (Debug)
            {
                return @"{
                      ""symbol"" : ""AAPL"",
                      ""historical"" : [ {
                        ""date"" : ""2021-03-24"",
                        ""open"" : 122.82,
                        ""high"" : 122.9,
                        ""low"" : 120.065,
                        ""close"" : 120.09,
                        ""adjClose"" : 120.09,
                        ""volume"" : 8.92177E7,
                        ""unadjustedVolume"" : 8.92177E7,
                        ""change"" : -2.73,
                        ""changePercent"" : -2.223,
                        ""vwap"" : 121.01833,
                        ""label"" : ""March 24, 21"",
                        ""changeOverTime"" : -0.02223
                      }, {
                        ""date"" : ""2021-03-23"",
                        ""open"" : 123.330002,
                        ""high"" : 124.239998,
                        ""low"" : 122.139999,
                        ""close"" : 122.540001,
                        ""adjClose"" : 122.540001,
                        ""volume"" : 9.51957E7,
                        ""unadjustedVolume"" : 9.51957E7,
                        ""change"" : -0.79,
                        ""changePercent"" : -0.641,
                        ""vwap"" : 122.97333,
                        ""label"" : ""March 23, 21"",
                        ""changeOverTime"" : -0.00641
                      }, {
                        ""date"" : ""2021-03-22"",
                        ""open"" : 120.330002,
                        ""high"" : 123.870003,
                        ""low"" : 120.260002,
                        ""close"" : 123.389999,
                        ""adjClose"" : 123.389999,
                        ""volume"" : 1.119123E8,
                        ""unadjustedVolume"" : 1.119123E8,
                        ""change"" : 3.06,
                        ""changePercent"" : 2.543,
                        ""vwap"" : 122.50667,
                        ""label"" : ""March 22, 21"",
                        ""changeOverTime"" : 0.02543
                      }, {
                        ""date"" : ""2021-03-19"",
                        ""open"" : 119.900002,
                        ""high"" : 121.43,
                        ""low"" : 119.68,
                        ""close"" : 119.989998,
                        ""adjClose"" : 119.989998,
                        ""volume"" : 1.855495E8,
                        ""unadjustedVolume"" : 1.855495E8,
                        ""change"" : 0.09,
                        ""changePercent"" : 0.075,
                        ""vwap"" : 120.36667,
                        ""label"" : ""March 19, 21"",
                        ""changeOverTime"" : 7.5E-4
                      } ]
                    }";
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["from"] = startDate.ToString("yyyy-MM-dd");
            query["to"] = DateTime.Today.ToString("yyyy-MM-dd");
            query["apikey"] = APIKey;
            var responseTask = m_client.GetAsync("/api/v3/historical-price-full/" + stockIndex + "?" + query.ToString());
            await responseTask;
            Trace.WriteLine("Response:" + responseTask.Result.StatusCode);
            return await responseTask.Result.Content.ReadAsStringAsync();
        }
    }
}
