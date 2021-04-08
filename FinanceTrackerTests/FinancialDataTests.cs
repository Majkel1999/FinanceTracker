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
        public void GetAllStockIndexes_SymbolDeserializeTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreEqual("AAPL", list[0].symbol);
            ApiRequest.Debug = false;
        }  
        
        [Test()]
        public void GetAllStockIndexes_NameDeserializeTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreEqual("Apple Inc.", list[0].name);
            ApiRequest.Debug = false;
        }
        [Test()]
        public void GetAllStockIndexes_PriceDeserializeTest()
        {
            ApiRequest.Debug = true;
            var list = FinancialData.GetAllStockIndexes().Result;
            Assert.AreNotEqual(0, list[0].price);
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
        public void GetStockIndexPrice_SymbolDeserializeTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetStockIndexPrice("AAPL").Result;
            Assert.AreEqual("AAPL",stock[0].symbol );
            ApiRequest.Debug = false;
        }
        [Test()]
        public void GetStockIndexPrice_PriceDeserializeTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetStockIndexPrice("AAPL").Result;
            Assert.AreEqual(120.58,stock[0].price);
            ApiRequest.Debug = false;
        }

        [Test()]
        public void GetHistoricalIndexDataTest_PriceDeserializeTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetHistoricalIndexData("AAPL").Result;
            Assert.AreEqual(4,stock.Count);
            ApiRequest.Debug = false;
        }

        [Test()]
        public void GetHistoricalIndexDataTest_SymbolDeserializeTest()
        {
            ApiRequest.Debug = true;
            var stock = FinancialData.GetHistoricalIndexData("AAPL").Result;
            Assert.AreEqual("AAPL",stock[0].symbol);
            ApiRequest.Debug = false;
        }
    }

}