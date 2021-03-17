using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<StockIndex> stockIndexesList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DatabaseController dbController = new DatabaseController();
            if (dbController.IsEmptyDB())
            {
                dbController.FillDB();
            }
            stockIndexesList = dbController.stockIndexes.OrderBy(b => b.symbol).ToList();
            //ListOfStocks.ItemsSource = stockIndexesList;
            //ListOfStocks.DisplayMemberPath = "symbol";
            DataContext = this;

            //dbController.Add(new StockIndex { })
            
            //StockPrice price = ApiRequest.GetData("AAPL");
            //TextBlock.Text = price.Symbol + ":" + price.Price;
            //List<HistoricalIndexData> list = ApiRequest.GetHistoricalData("AAPL");

            //foreach (HistoricalIndexData stock in list)
            //{
            //    Trace.WriteLine(stock.symbol + ":" + stock.date + ":" + stock.price);
            //}
        }
    }
}
