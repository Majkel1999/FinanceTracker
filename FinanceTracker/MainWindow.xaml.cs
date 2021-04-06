using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using LiveCharts;
using System.Text.RegularExpressions;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DatabaseController dbController;

        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public LineSeries lineSeries { get; set; }

        public double buyPrice { get; set; }
        public string buyQuantity { get; set; }

        private List<StockIndex> m_stockIndexesList;
        public IEnumerable<StockIndex> stockIndexesList
        {
            get
            {
                if (searchText == null)
                {
                    return m_stockIndexesList;
                }
                return m_stockIndexesList.Where(x => x.symbol.ToUpper().StartsWith(searchText.ToUpper()));
            }
            set { m_stockIndexesList = (List<StockIndex>)value; }
        }

        private List<MyStock> m_myStockIndexesList;
        public IEnumerable<MyStock> myStockIndexesList
        {
            get
            {
                if (searchText_MyStock == null)
                {
                    return m_myStockIndexesList;
                }
                return m_myStockIndexesList.Where(x => x.symbol.ToUpper().StartsWith(searchText_MyStock.ToUpper()));
            }
            set
            {
                m_myStockIndexesList = (List<MyStock>)value;
                OnPropertyChanged("myStockIndexesList");
            }
        }

        private string m_searchText;
        public string searchText
        {
            get { return m_searchText; }
            set
            {
                m_searchText = value;
                OnPropertyChanged("searchText");
                OnPropertyChanged("stockIndexesList");
            }
        }
        private string m_searchText_MyStock;
        public string searchText_MyStock
        {
            get { return m_searchText_MyStock; }
            set
            {
                m_searchText_MyStock = value;
                OnPropertyChanged("searchText_MyStock");
                OnPropertyChanged("myStockIndexesList");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            dbController = new DatabaseController();
            if (!dbController.stockIndexes.Any())
            {
                dbController.FillStockIndexesTable();
            }
            stockIndexesList = dbController.stockIndexes.OrderBy(b => b.symbol).ToList();
            myStockIndexesList = dbController.myStocks.OrderBy(x => x.symbol).ToList();
            var ModelConfig = Mappers.Xy<HistoricalIndexData>()
                .X(model => model.date.Ticks)
                .Y(model => model.price);
            Charting.For<HistoricalIndexData>(ModelConfig);
            XFormatter = value => new DateTime((long)value).ToString("dd-MM-yy");
            YFormatter = value => value.ToString("0.00$");
            lineChart.Visibility = Visibility.Hidden;
            DataContext = this;
        }

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void ListOfStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGraphLine();
            buyQuantity = string.Empty;
            QuantityTextBox.Text = "0";
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            var item = (StockIndex)ListOfStocks.SelectedItem;
            Thread thread = new Thread(() =>
            {
                if (item != null)
                {
                    DatabaseController databaseController = new DatabaseController();
                    databaseController.UpdateHistoricalTable(item.symbol);
                }
                Dispatcher.Invoke(() =>
                {
                    UpdateGraphLine();
                    UpdateButton.IsEnabled = true;
                });
            });
            thread.Start();
        }
        private void UpdateGraphLine()
        {
            var item = (StockIndex)ListOfStocks.SelectedItem;
            if (item != null)
            {

                var historicalData = dbController.historicalIndexes.Where(x => x.symbol == item.symbol).Where(x => x.date > DateTime.Now.AddYears(-1)).OrderByDescending(x => x.date);
                if (historicalData.Any())
                {
                    lineSeries = new LineSeries();
                    lineSeries.Values = new ChartValues<HistoricalIndexData>();
                    lineSeries.Values.AddRange(historicalData);
                    lineSeriesChart.Values = lineSeries.Values;
                    lineChart.Visibility = Visibility.Visible;
                }
                else
                {
                    lineChart.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                lineChart.Visibility = Visibility.Hidden;
            }
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = buyQuantity;

            if (!Regex.IsMatch(input, @"^[1-9][0-9]*$")) //only numbers not starting with 0
            {
                QuantityTextBox.Text = string.Empty; //wywala binding error ale apka działa idealnie? 
                buyPrice = 0;
                OnPropertyChanged("buyPrice");
                OnPropertyChanged("buyQuantity");
            }
            else
            {
                var item = (StockIndex)ListOfStocks.SelectedItem;
                if (item != null)
                {
                    buyPrice = item.price * ulong.Parse(buyQuantity);
                    OnPropertyChanged("buyPrice");
                }
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (StockIndex)ListOfStocks.SelectedItem;
            if (item != null)
            {
                ulong quantity;
                try
                {
                    quantity = ulong.Parse(buyQuantity);
                }
                catch
                {
                    quantity = 0;
                }
                if (buyPrice != 0 && quantity != 0)
                {

                    var myStock = new MyStock
                    {
                        symbol = item.symbol,
                        indexPrice = item.price,
                        transactionDate = DateTime.Now,
                        transactionVolume = ulong.Parse(buyQuantity),
                        profit = 0
                    };
                    dbController.Add(myStock);
                    dbController.SaveChanges();
                    m_myStockIndexesList.Add(dbController.myStocks.OrderBy(x => x.transactionDate).Last());
                    OnPropertyChanged("myStockIndexesList");
                }
            }
            buyQuantity = string.Empty;
            OnPropertyChanged("buyQuantity");
        }

        private void ListOfMyStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListOfMyStocks.SelectedIndex != -1)
            {
                DatabaseController databaseController = new DatabaseController();
                var item = (MyStock)ListOfMyStocks.SelectedItem;
                var currentItem = databaseController.stockIndexes.Where(x => x.symbol == item.symbol).FirstOrDefault();
                CurrentPrice.Text = "Current Price : " + currentItem.price.ToString("0.00");
                MyPrice.Text = "Bought for : " + item.indexPrice.ToString("0.00");
                MyProfit.Text = "Profit : " + ((currentItem.price - item.indexPrice) * item.transactionVolume).ToString("+0.00;-0.00;0");
            }
        }

        private void UpdateAllButton_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                DatabaseController databaseController = new DatabaseController();
                databaseController.UpdateStockIndexesTable();
                stockIndexesList = databaseController.stockIndexes.OrderBy(b => b.symbol).ToList();
                Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged("stockIndexesList");
                    OnPropertyChanged("myStockIndexesList");
                });
            }).Start();
        }
    }
}
