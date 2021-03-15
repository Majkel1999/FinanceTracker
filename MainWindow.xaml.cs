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
        public MainWindow()
        {
            InitializeComponent();
            //StockPrice price = ApiRequest.GetData("AAPL");
            //TextBlock.Text = price.Symbol + ":" + price.Price;
            //List<StockIndex> list = ApiRequest.GetStockIndexes();
            //foreach(StockIndex stock in list)
            //{
            //    Trace.WriteLine(stock.Symbol + ":" + stock.Name + ":" + stock.Price);
            //}
        }
    }
}
