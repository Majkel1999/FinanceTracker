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

        public bool IsEmptyDB()
        {
            if ((!this.stockIndexes.Any()) && (!this.historicalIndexes.Any()) && (!this.myStocks.Any()))
                return true;
            return false;
        }

        public void FillDB()
        {
            List<StockIndex> stockIndexesList = ApiRequest.GetStockIndexes();
            uint i = 0;
            foreach (StockIndex stock in stockIndexesList)
            {
                List<HistoricalIndexData> historicalData = ApiRequest.GetHistoricalData(stock.symbol);
                this.Add(stock);
                foreach (HistoricalIndexData historicalIndexData in historicalData)
                {
                    historicalIndexData.ID = i;
                    this.Add(historicalIndexData);
                    ++i;
                }
                if (i > 1)
                    break;
            }
            this.SaveChanges();
        }

        //public bool IsOld()
        //{
        //    List<StockIndex> stockIndexes = ApiRequest.GetStockIndexes();

        //    return false;
        //}

        //public void UpdateDB()
        //{
        //    List<StockIndex> stockIndexes = ApiRequest.GetStockIndexes();

        //    foreach (StockIndex stock in stockIndexes)
        //    {
        //        List<HistoricalIndexData> historicalData = ApiRequest.GetHistoricalData(stock.symbol);
        //        this.Add(stock);
        //        foreach (HistoricalIndexData historicalIndexData in historicalData)
        //        {
        //            this.Add(historicalIndexData);
        //        }
        //    }
        //    this.SaveChanges();
        //}

    }
}
