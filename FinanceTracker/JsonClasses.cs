using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker
{
    /// <summary>
    /// Klasa pomocnicza do parsowania JSON odbieranego z zewnętrzengo API
    /// symbol - skrót nazwy indeksu giełdowego
    /// price - cena indeksu giełdowego
    /// volume - ilość dostępnych udziałów
    /// </summary>
    public class StockPrice
    {
        public string symbol;
        public double price;
        public ulong volume;

    }

    /// <summary>
    /// Klasa pomocnicza do parsowania JSON odbieranego z zewnętrznego API
    /// Klasa służy do przechowywania pojedynczych danych historycznych
    /// date - informuje, z jakiego dnia pochodzą dane
    /// high - najwyższa cena indeksu giełdowego z danego dnia 
    /// volume - ilość udziałów dostęnych na giełdzie
    /// changeOverTime - zmiana w stosunku do poprzedniej wartości ceny
    /// </summary>
    public class HistoricalData
    {
        public DateTime date;
        public double high;
        public double volume;
        public double changeOverTime;
    }

    /// <summary>
    /// Klasa pomocnicza do parsowania JSON odbieranego z zewnętrznego API
    /// Klasa służy do przechowywania danych historycznych dla danego indeksu
    /// symbol - skrót nazwy indeksu giełdowego
    /// historical - zbiór danych HistoricalData przechowujący informacje o cenach oraz ilościach udziałów danego indeksu
    /// </summary>
    public class HistoricalDataList
    {
        public string symbol;
        public List<HistoricalData> historical =new List<HistoricalData>();
    }
}
