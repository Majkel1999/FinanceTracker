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
using System.Windows.Media;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
            new Thread(() =>
            {
                DatabaseSetup();
                ChartSetup();
                InitialSetup();
            }).Start();
        }

        private void DatabaseSetup()
        {
            DatabaseController dbController = new DatabaseController();
            if (!dbController.stockIndexes.Any())
            {
                dbController.FillStockIndexesTable();
            }
            stockIndexesList = dbController.stockIndexes.OrderBy(b => b.symbol).ToList();

            if (!dbController.historicalIndexes.Where(x => x.symbol == stockIndexesList.First().symbol).Any())
            {
                dbController.FillHistoricalTable(stockIndexesList.First().symbol);
            }
            myStockIndexesList = dbController.myStocks.OrderBy(x => x.symbol).ToList();
        }

        private void ChartSetup()
        {
            var HistoricalPriceMapper = Mappers.Xy<HistoricalIndexData>()
                .X(model => model.date.Ticks)
                .Y(model => model.price);
            Charting.For<HistoricalIndexData>(HistoricalPriceMapper);

            var MyStockProfitMapper = Mappers.Xy<MyStockProfit>()
                .X(model => model.date.Ticks)
                .Y(model => model.profit)
                .Stroke((model) => model.profit >= 0 ? Brushes.Green : Brushes.Red)
                .Fill((model) => model.profit >= 0 ? Brushes.Green : Brushes.Red);
            Charting.For<MyStockProfit>(MyStockProfitMapper);

            XFormatter = value => new DateTime((long)value).ToString("dd-MM-yy");
            YFormatter = value => value.ToString("0.00$");
        }

        private void InitialSetup()
        {
            UpdateMyStockDetailData();
            Dispatcher.Invoke(() =>
            {
                ListOfRealStocks.SelectedIndex = 0;
                if (m_myStockIndexesList.Count > 0)
                {
                    ListOfMyStocks.SelectedIndex = 0;
                }
                else
                {
                    MyStockTab.IsEnabled = false;
                }
                DataContext = this;
            });
        }

        private void UpdateMyStockDetailData()
        {
            DatabaseController databaseController = new DatabaseController();
            foreach (MyStock stock in databaseController.myStocks)
            {
                var currentItem = databaseController.stockIndexes.Where(x => x.symbol == stock.symbol).FirstOrDefault();
                stock.profit = ((currentItem.price - stock.indexPrice) * stock.transactionVolume);
                databaseController.SaveChanges();
            }
        }

        private void UpdateHistoricalIndexGraph()
        {
            DatabaseController dbController = new DatabaseController();
            var item = (StockIndex)ListOfRealStocks.SelectedItem;
            if (item != null)
            {
                var historicalData = dbController.historicalIndexes.Where(x => x.symbol == item.symbol).Where(x => x.date > DateTime.Now.AddYears(-1)).OrderByDescending(x => x.date);
                if (historicalData.Any())
                {
                    CreateGraphData(historicalData);
                    ShowStockGraph();
                }
                else
                {
                    HideStockGraph();
                }
            }
            else
            {
                HideStockGraph();
            }
        }

        private void UpdateMyStockProfitGraph(MyStock myStock)
        {
            DatabaseController databaseController = new DatabaseController();

            bool isMyStockProfitUpdated = databaseController.myStockProfits
                .Where(x => x.symbol == myStock.symbol)
                .Where(x => x.date.Date == DateTime.Today.Date.AddDays(-1))
                .Any();

            if (!isMyStockProfitUpdated)
            {
                UpdateMyStockProfit(myStock);
            }

            Dispatcher.Invoke(() =>
            {
                LineSeries stepLineSeries = new LineSeries();
                stepLineSeries.Values = new ChartValues<MyStockProfit>();
                stepLineSeries.Values.AddRange(databaseController.myStockProfits.Where(x => x.transactionID == myStock.transactionID));
                ProfitChartSeries.Values = stepLineSeries.Values;
            });
        }

        private void UpdateMyStockProfit(MyStock myStock)
        {
            DatabaseController databaseController = new DatabaseController();
            DateTime newestProfit;
            if (databaseController.myStockProfits.Where(x => x.transactionID == myStock.transactionID).Any())
            {
                newestProfit = databaseController.myStockProfits
                    .Where(x => x.symbol == myStock.symbol)
                    .OrderByDescending(x => x.date)
                    .FirstOrDefault().date;
            }
            else
            {
                newestProfit = myStock.transactionDate;
            }

            bool NeedToUpdateHistorical = !databaseController.historicalIndexes
                .Where(x => x.symbol == myStock.symbol)
                .Where(x => x.date.Date == DateTime.Today.AddDays(-1))
                .Any();

            if (NeedToUpdateHistorical && myStock.transactionDate < DateTime.Today)
            {
                databaseController.FillHistoricalTable(myStock.symbol);
            }



            var list = databaseController.historicalIndexes
                .Where(x => x.symbol == myStock.symbol)
                .Where(x => x.date > newestProfit)
                .OrderByDescending(x => x.date)
                .ToList();

            foreach (var l in list)
            {
                databaseController.myStockProfits.Add(new MyStockProfit
                {
                    date = l.date,
                    profit = l.price - myStock.indexPrice,
                    transactionID = myStock.transactionID,
                    symbol = myStock.symbol
                });
                databaseController.SaveChanges();
            }
        }

        private void CreateGraphData(IOrderedQueryable<HistoricalIndexData> historicalData)
        {
            lineSeries = new LineSeries();
            lineSeries.Values = new ChartValues<HistoricalIndexData>();
            lineSeries.Values.AddRange(historicalData);
            RealStockChartSeries.Values = lineSeries.Values;
        }

        private void ShowStockGraph()
        {
            RealStockChart.Visibility = Visibility.Visible;
            noDataLabel.Visibility = Visibility.Hidden;
            noDataBorder.Visibility = Visibility.Hidden;
        }

        private void HideStockGraph()
        {
            RealStockChart.Visibility = Visibility.Hidden;
            noDataLabel.Visibility = Visibility.Visible;
            noDataBorder.Visibility = Visibility.Visible;
        }

        private void AddNewBoughtStock(MyStock myStock)
        {
            DatabaseController dbController = new DatabaseController();
            dbController.Add(myStock);
            dbController.SaveChanges();
            m_myStockIndexesList.Add(dbController.myStocks.OrderBy(x => x.transactionDate).Last());
            MyStockTab.IsEnabled = true;
            UpdateMyStockDetailData();
            OnPropertyChanged("myStockIndexesList");
        }

        private void RemoveSoldStock(MyStock myStock, int quantity)
        {
            DatabaseController dbController = new DatabaseController();
            if (myStock.transactionVolume == quantity)
            {
                m_myStockIndexesList.Remove(m_myStockIndexesList.Find(x => x.transactionID == myStock.transactionID));
                dbController.myStockProfits.RemoveRange(dbController.myStockProfits.Where(x => x.transactionID == myStock.transactionID));
                dbController.Remove(myStock);
                if (m_myStockIndexesList.Count > 0)
                {
                    ListOfMyStocks.SelectedIndex = 0;
                }
                else
                {
                    RealStockTab.IsSelected = true;
                    MyStockTab.IsEnabled = false;
                    ListOfMyStocks.SelectedIndex = -1;
                }
            }
            else
            {
                myStock.transactionVolume -= quantity;
                dbController.Update(myStock);
            }

            dbController.SaveChanges();
            OnPropertyChanged("myStockIndexesList");
            UpdateMyStockDetailData();
            UpdateSelectedMyStock();
        }

        private void UpdateSelectedMyStock()
        {
            DatabaseController databaseController = new DatabaseController();
            if (ListOfMyStocks.SelectedIndex != -1)
            {
                var item = (MyStock)ListOfMyStocks.SelectedItem;
                var currentItem = databaseController.stockIndexes.Where(x => x.symbol == item.symbol).FirstOrDefault();
                CurrentPrice.Text = currentItem.price.ToString("0.00");
                MyPrice.Text = item.indexPrice.ToString("0.00");
                MyProfit.Text = item.profit.ToString("+0.00;-0.00;0");
                new Thread(() =>
                {
                    UpdateMyStockProfitGraph(item);
                }).Start();
            }
        }
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void ListOfStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHistoricalIndexGraph();
            buyQuantity = string.Empty;
            BuyQuantityTextBox.Text = "0";
        }

        private void ListOfMyStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedMyStock();
        }
        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = buyQuantity;

            if (!Regex.IsMatch(input, @"^[1-9][0-9]*$")) //only numbers not starting with 0
            {
                BuyQuantityTextBox.Text = string.Empty; //wywala binding error ale apka działa idealnie? 
                buyPrice = 0;
                OnPropertyChanged("buyPrice");
                OnPropertyChanged("buyQuantity");
            }
            else
            {
                var item = (StockIndex)ListOfRealStocks.SelectedItem;
                if (item != null)
                {
                    buyPrice = item.price * ulong.Parse(buyQuantity);
                    OnPropertyChanged("buyPrice");
                }
            }
        }

        private void SellQuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = SellQuantityTextBox.Text;

            if (!Regex.IsMatch(input, @"^[1-9][0-9]*$")) //only numbers not starting with 0
            {
                SellQuantityTextBox.Text = string.Empty;
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (StockIndex)ListOfRealStocks.SelectedItem;
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
                        transactionDate = DateTime.Now.AddYears(-1),
                        transactionVolume = ulong.Parse(buyQuantity),
                        profit = 0
                    };
                    AddNewBoughtStock(myStock);
                }
            }
            buyQuantity = string.Empty;
            OnPropertyChanged("buyQuantity");
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (MyStock)ListOfMyStocks.SelectedItem;
            if (SellQuantityTextBox.Text.Length > 0)
            {
                int quantity = int.Parse(SellQuantityTextBox.Text);
                if (item != null && quantity != 0)
                {
                    if (item.transactionVolume < quantity)
                    {
                        MessageBox.Show("You do not have " + quantity + " stocks to sell.\nPlease enter " + item.transactionVolume + " or less.");
                    }
                    else
                    {
                        var result = MessageBox.Show(
                            "Are you sure you want to sell " + quantity + " " + item.symbol + "?\nProfit:" + (item.profit / item.transactionVolume * quantity).ToString("0.00$")
                            , "Confirmation", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            RemoveSoldStock(item, quantity);
                        }
                    }
                }
            }
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            var item = (StockIndex)ListOfRealStocks.SelectedItem;
            Thread thread = new Thread(() =>
            {
                if (item != null)
                {
                    DatabaseController databaseController = new DatabaseController();
                    databaseController.UpdateHistoricalTable(item.symbol);
                }
                Dispatcher.Invoke(() =>
                {
                    if (item == (StockIndex)ListOfRealStocks.SelectedItem)
                        UpdateHistoricalIndexGraph();
                    UpdateButton.IsEnabled = true;
                });
            });
            thread.Start();
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
                    UpdateMyStockDetailData();
                    OnPropertyChanged("stockIndexesList");
                    OnPropertyChanged("myStockIndexesList");
                });
            }).Start();
        }
    }
}
