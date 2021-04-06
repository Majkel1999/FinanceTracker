using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Tests
{
    [TestClass()]
    public class DatabaseControllerTests
    {
        [TestMethod()]
        public void FillStockIndexesTableTest()
        {
            ApiRequest.Debug = true;
            DatabaseController dbController = new DatabaseController(true);
            Assert.AreEqual(0, dbController.stockIndexes.Count());
            dbController.FillStockIndexesTable();
            Assert.AreEqual(2, dbController.stockIndexes.Count());
            Assert.AreEqual(0.58, dbController.stockIndexes.Where(x => x.symbol == "AAPL").FirstOrDefault().price);

        }

        [TestMethod()]
        public void FillHistoricalTableTest()
        {
            ApiRequest.Debug = true;
            DatabaseController dbController = new DatabaseController(true);
            Assert.AreEqual(0, dbController.stockIndexes.Count());
            dbController.FillHistoricalTable("AAPL");
            Assert.AreEqual(4, dbController.historicalIndexes.Where(x => x.symbol == "AAPL").Count());
            Assert.AreEqual(121.43, dbController.historicalIndexes.Where(x => x.symbol == "AAPL").Where(x => x.date == new DateTime(2021, 3, 19)).FirstOrDefault().price);
        }
    }
}