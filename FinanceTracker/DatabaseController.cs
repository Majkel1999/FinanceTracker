using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace FinanceTracker
{
    public class DatabaseController : DbContext
    {
        public DbSet<StockIndex> stockIndexes { get; set; }
        public DbSet<HistoricalIndexData> historicalIndexes { get; set; }
        public DbSet<MyStock> myStocks { get; set; }

        public DatabaseController()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=.\database.db").EnableSensitiveDataLogging();

        public void FillStockIndexesTable()
        {
            List<StockIndex> stockIndexesList = ApiRequest.GetStockIndexes();
            foreach (StockIndex stock in stockIndexesList)
            {
                this.Add(stock);
            }
            this.SaveChanges();
        }

        public void FillHistoricalTable(string stockIndex)
        {
            List<HistoricalIndexData> historicalData = ApiRequest.GetHistoricalData(stockIndex);
            foreach (HistoricalIndexData data in historicalData)
            {
                this.Add(data);
            }
            this.SaveChanges();
        }

        public void UpdateHistoricalTable(string stockIndex)
        {
            DateTime date;
            if (historicalIndexes.Where(x => x.symbol == stockIndex).Any())
            {
                var lastIndex = historicalIndexes.Where(b => b.symbol == stockIndex).OrderBy(b => b.date).Last();
                date = lastIndex.date;
            }
            else
            {
                date = DateTime.Today.AddYears(-1);
            }

            List<HistoricalIndexData> historicalData = ApiRequest.GetHistoricalData(stockIndex, date);
            foreach (HistoricalIndexData data in historicalData)
            {
                this.Add(data);
            }
            var index = stockIndexes.Where(x => x.symbol == stockIndex).FirstOrDefault();
            index.price = historicalData[0].price;
            this.SaveChanges();
        }

        public void UpdateStockIndexesTable()
        {
            List<StockIndex> stockIndexesList = ApiRequest.GetStockIndexes();
            foreach (StockIndex stock in stockIndexesList)
            {
                var index = stockIndexes.Where(x => x.symbol == stock.symbol).FirstOrDefault();
                if (index == null)
                {
                    this.Add(stock);
                }
                else
                {
                    index.price = stock.price;
                }
            }
            this.SaveChanges();
        }
    }
}
