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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using LiveCharts;

namespace FinanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<StockIndex> m_stockIndexesList;
         
        private DatabaseController dbController;

        public Func<double,string> Formatter { get; set; }
        public LineSeries lineSeries { get; set; }

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
        private string m_searchText;
        public string searchText
        {
            get { return m_searchText; }
            set { m_searchText = value;
                OnPropertyChanged("searchText");
                OnPropertyChanged("stockIndexesList");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            dbController = new DatabaseController();
            if (!dbController.stockIndexes.Any())
            {
                dbController.FillStockIndexesTable();
            }
            stockIndexesList = dbController.stockIndexes.OrderBy(b => b.symbol).ToList();

            //ListOfStocks.ItemsSource = stockIndexesList;
            //ListOfStocks.DisplayMemberPath = "symbol";


            var item = (StockIndex)ListOfStocks.SelectedItem;
            
            var ModelConfig = Mappers.Xy<HistoricalIndexData>()
                .X(model => model.date.Ticks)
                .Y(model => model.price);
            Charting.For<HistoricalIndexData>(ModelConfig);

            Formatter = value => new DateTime((long)value).ToString("dd-MM-yy");
            DataContext = this;


        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void ListOfStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                }
            }
        }
    }
}
