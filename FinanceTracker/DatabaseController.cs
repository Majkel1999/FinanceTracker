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
            var lastIndex = historicalIndexes.Where(b => b.symbol == stockIndex).OrderBy(b => b.date).Last();
            List<HistoricalIndexData> historicalData = ApiRequest.GetHistoricalData(stockIndex,lastIndex.date.AddDays(1));
            foreach (HistoricalIndexData data in historicalData)
            {
                this.Add(data);
            }
            this.SaveChanges();
        }

        public void UpdateStockIndexesTable()
        {
            HistoricalIndexData lastIndex;
            foreach (StockIndex stock in stockIndexes)
            {
                lastIndex = historicalIndexes.Where(b => b.symbol == stock.symbol).OrderBy(b => b.date).Last();
                stock.price = lastIndex.price;
            }
        }
    }
}
