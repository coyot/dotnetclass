using System;
using System.Collections.Generic;
using System.Threading;

namespace Stock.Ticker
{
    public interface IStockTicker
    {
        IEnumerable<Index> AllIndexes { get; }
            
        IEnumerable<Stock> AllStocks { get; }

        IDisposable Start();

        event EventHandler<StockChangedEventArgs> StockChanged;
    }
}
