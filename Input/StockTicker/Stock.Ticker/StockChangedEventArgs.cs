using System;

namespace Stock.Ticker
{
    public class StockChangedEventArgs : EventArgs
    {
        public Stock Product { get; internal set; }

        public double CurrentValue { get; internal set; }

        public DateTimeOffset Time { get; internal set; }
    }
}