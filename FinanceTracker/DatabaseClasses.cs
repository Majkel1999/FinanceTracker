using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker
{
    /// <summary>
    /// Klasa przechowaująca podstawowe dane o indeksie giełdowym:
    /// symbol - skrót nazwy firmy
    /// name - pełna nazwa firmy
    /// price - cena udziałów danej firmy
    /// </summary>

    public class StockIndex
    {
        [Key] public string symbol { get; set; }
        public string name { get; set; }
        public double price { get; set; }
    }

    /// <summary>
    /// Klasa przechowująca fragment danych historycznych indeksu giełdowego
    /// symbol - skrót nazwy firmy
    /// date - data dani z którego pochodzą dane historyczne
    /// price - cena udziałów z danego dnia
    /// volume - ilość udziałów dostęnych na giełdzie
    /// changeOverTime - zmiana ceny w stosunku do poprzedniej wartośći
    /// </summary>
    public class HistoricalIndexData
    {
        [Key] public int ID { get; set; }
        public string symbol { get; set; }
        public DateTime date { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public double changeOverTime { get; set; }
    }
    /// <summary>
    /// Klasa przechowująca transakcje "zakupu" danego indeksu giełdowego
    /// transactionID - numer transakcji
    /// symbol - skrót nazwy firmy
    /// transactionDate - data zakupu udziałów
    /// transactionVolume - ilość zakupionych udziałów
    /// indexPrice - cena po jakiej zostały zakupione dane udziały
    /// profit - zysk/strata w stosunku do ceny bieżącej udziałów
    /// </summary>
    public class MyStock
    {
        [Key] public int transactionID { get; set; }
        public string symbol { get; set; }
        public DateTime transactionDate { get; set; }
        public double transactionVolume { get; set; }
        public double indexPrice { get; set; }
        public double profit { get; set; }

    }

    public class MyStockProfit
    {
        [Key] public int ID { get; set; }
        public int transactionID { get; set; }
        public string symbol { get; set; }
        public DateTime date { get; set; }
        public double profit { get; set; }
    }

}
