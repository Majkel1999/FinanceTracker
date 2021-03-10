using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
/* Api Key AlphaVantage
* 8PXA60GU992EV4KA 
*/
/* Api Key financialModelingPrep
 * b9d32703beec60431f239bcfae715d7f
 */
namespace FinanceTracker
{
    static class ApiRequest
    {
        const string APIKey = "b9d32703beec60431f239bcfae715d7f";
        static HttpClient client = new HttpClient();

        public static string GetData()
        {
            client.BaseAddress = new Uri("https://financialmodelingprep.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = "b9d32703beec60431f239bcfae715d7f";
            HttpResponseMessage response = client.GetAsync("/api/v3/quote-short/AAPL?" + query.ToString()).Result;
            return response.Content.ReadAsStringAsync().Result;

        }
    }
}
