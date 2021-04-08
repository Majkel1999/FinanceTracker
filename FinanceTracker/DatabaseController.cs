using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace FinanceTracker
{
    /// <summary>
    /// Klasa obsługująca bazę danych.
    /// </summary>
    public class DatabaseController : DbContext
    {
        private bool Debug = false;
        /// <summary>
        /// Tabela z bazy danych zawierająca skróty nazw oraz nazwy wszystkich indeksów giełdowych wraz z ich ceną giełdową. 
        /// </summary>
        public DbSet<StockIndex> stockIndexes { get; set; }

        /// <summary>
        /// Tabela z bazy danych zawierająca dane historyczne (do roku w tył) indeksów giełdowych. 
        /// </summary>
        public DbSet<HistoricalIndexData> historicalIndexes { get; set; }

        /// <summary>
        /// Tabela z bazy danych zawierająca informację o "zakupionych" przez użytkownika udziałach. 
        /// </summary>
        public DbSet<MyStock> myStocks { get; set; }

        public DbSet<MyStockProfit> myStockProfits { get; set; }

        /// <summary>
        /// Konstruktor klasy 
        /// </summary>
        public DatabaseController(bool debug = false)
        {
            Debug = debug;
            Database.OpenConnection();
            Database.EnsureCreated();
        }

        /// <summary>
        /// Nadpisana metoda konfiguracji bazy danych. 
        /// </summary>
        /// <param name="options">Parametry konfiguracyjne bazy danych</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (Debug)
            {
                options.UseSqlite(@"Data Source=file::memory:?cache=shared").EnableSensitiveDataLogging();
            }
            else
            {
                options.UseSqlite(@"Data Source=.\database.db").EnableSensitiveDataLogging();
            }
        }

        /// <summary>
        /// Nadpisana metoda usuwająca bazę danych.
        /// </summary>
        public override void Dispose()
        {
            Database.CloseConnection();
            base.Dispose();
        }
        /// <summary>
        /// Metoda uzupełniająca tabelę z bazy danych przechowującą informacje o cenie i oraz pełnej nazwie indeksu giełdowego.
        /// </summary>
        public void FillStockIndexesTable()
        {
            List<StockIndex> stockIndexesList = FinancialData.GetAllStockIndexes().Result;
            foreach (StockIndex stock in stockIndexesList)
            {
                this.Add(stock);
            }
            this.SaveChanges();
        }

        /// <summary>
        /// Metoda uzupełniająca dane historyczne podanego w argumencie indeksu giełdowego.
        /// </summary>
        /// <param name="stockIndex">Symbol indeksu giełdowego, którego dane historyczne są do uzupełnienia</param>
        public void FillHistoricalTable(string stockIndex)
        {
            List<HistoricalIndexData> historicalData = FinancialData.GetHistoricalIndexData(stockIndex).Result;
            foreach (HistoricalIndexData data in historicalData)
            {
                this.Add(data);
            }
            this.SaveChanges();
        }

        /// <summary>
        /// Metoda aktualizująca dane historyczne w bazie danych dla zadanego indeksu giełdowego.
        /// </summary>
        /// <param name="stockIndex">Symbol indeksu giełdowego, dla którego należy zaktualizować dane historyczne</param>
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
            List<HistoricalIndexData> historicalData = FinancialData.GetHistoricalIndexData(stockIndex, date).Result;
            foreach (HistoricalIndexData data in historicalData)
            {
                this.Add(data);
            }
            if (historicalData.Count >0)
            {
                var index = stockIndexes.Where(x => x.symbol == stockIndex).FirstOrDefault();
                index.price = historicalData[0].price;
            }
            this.SaveChanges();
        }

        /// <summary>
        /// Metoda aktualizująca informacje o spisie indeksów giełdowych.
        /// </summary>
        public void UpdateStockIndexesTable()
        {
            List<StockIndex> stockIndexesList = FinancialData.GetAllStockIndexes().Result;
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
