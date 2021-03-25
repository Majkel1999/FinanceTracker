using FinanceTracker;
using NUnit.Framework;

namespace FinanceTracker.Tests
{
    [TestFixture()]
    public class FinancialDataTests
    {
        [Test()]
        public void GetAllStockIndexes_CountTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreEqual(list.Count, 2);
            ApiRequest.Debug = false;
        }
        [Test()]
        public void GetAllStockIndexes_StringDeserializeTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreEqual("AAPL", list[0].symbol);
            Assert.AreEqual("Apple Inc.", list[0].name);
            Assert.AreEqual("CMCSA", list[1].symbol);
            Assert.AreEqual("Comcast Corp", list[1].name);
            ApiRequest.Debug = false;
        }
        [Test()]
        public void GetAllStockIndexes_PriceDeserializeTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreNotEqual(0, list[0].price);
            Assert.AreNotEqual(0, list[1].price);
            ApiRequest.Debug = false;
        }

        [Test()]
        public void GetStockIndexPrice_LengthTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetStockIndexPrice("AAPL").Result;
            Assert.AreEqual(1, stock.Length);
            ApiRequest.Debug = false;
        }
        [Test()]
        public void GetStockIndexPrice_DeserializeTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetStockIndexPrice("AAPL").Result;
            Assert.AreEqual("AAPL",stock[0].symbol );
            Assert.AreEqual(120.58,stock[0].price);
            ApiRequest.Debug = false;
        }

        [Test()]
        public void GetHistoricalIndexDataTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetHistoricalIndexData("AAPL").Result;
            Assert.AreEqual(4,stock.Count);
            Assert.AreEqual("AAPL",stock[0].symbol);
            ApiRequest.Debug = false;
        }

        [Test()]
        public void GetHistoricalIndexDataTest1()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetHistoricalIndexData("AAPL", System.DateTime.Now).Result;
            Assert.AreEqual(4,stock.Count);
            Assert.AreEqual("AAPL",stock[0].symbol);
            ApiRequest.Debug = false;
        }
    }

}