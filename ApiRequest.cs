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
    public class StockPrice
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
        public ulong Volume { get; set; }

    }

    public class StockIndex
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    static class ApiRequest
    {
        const string APIKey = "b9d32703beec60431f239bcfae715d7f";
        static HttpClient client = new HttpClient();

        static ApiRequest()
        {
            client.BaseAddress = new Uri("https://financialmodelingprep.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static StockPrice GetData(string stockIndex)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            HttpResponseMessage response = client.GetAsync("/api/v3/quote-short/" + stockIndex + "?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            result = result.TrimStart('[');
            result = result.TrimEnd(']');
            Trace.Write(result);
            return JsonConvert.DeserializeObject<StockPrice>(result);

        }
        public static List<StockIndex> GetStockIndexes()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = APIKey;
            HttpResponseMessage response = client.GetAsync("/api/v3/available-traded/list?" + query.ToString()).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            List<StockIndex> indexList = JsonConvert.DeserializeObject<StockIndex[]>(result).ToList();
            indexList.RemoveAll(s => s.Symbol.Contains('.'));
            return indexList;
            
        }
    }
}
