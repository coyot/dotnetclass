using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockTicker
{
    class Program
    {
        static void Main()
        {
            var stockTicker = Stock.Ticker.StockTicker.CreateInstance("StockData.txt");
            foreach (var stock in stockTicker.AllStocks)
            {
                Console.WriteLine("Stock: {0} <{1}>", stock.Symbol, stock.Index.IndexName);
            }

            stockTicker.StockChanged += (sender, args) =>
            {
               Console.WriteLine("{0}: New price received for {1} -> {2} [Thread: {3}]", args.Time, args.Product.Symbol, args.CurrentValue, Thread.CurrentThread.ManagedThreadId);
            };

            using (stockTicker.Start())
            {
                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
