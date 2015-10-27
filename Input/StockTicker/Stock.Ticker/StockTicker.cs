using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stock.Ticker
{
    public class StockTicker : IStockTicker
    {
        private static StockTicker _instance;
        private readonly List<Index> _indexes = new List<Index>();
        private readonly List<Tuple<Stock, Tuple<int, int>>> _priceRange = new List<Tuple<Stock, Tuple<int, int>>>();
        private readonly Random _rnd = new Random();
        private Thread _thread;

        private StockTicker()
        {
        }

        public static StockTicker CreateInstance(string configurationFilePath)
        {
            if (null != _instance)
                return _instance;

            if (string.IsNullOrEmpty(configurationFilePath))
                throw new ArgumentException("configurationFilePath");

            if (!File.Exists(configurationFilePath))
                throw new FileNotFoundException("Can't load configuration file", configurationFilePath);

            _instance = new StockTicker();

            Trace.WriteLine(string.Format("Loading ticker configuration from " + configurationFilePath));

            var content = File.ReadAllLines(configurationFilePath);
            var indexes = new Dictionary<string, Index>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in content)
            {
                Trace.WriteLine(string.Format("Parsing item {0}", line));
                var data = line.Split(',');
                if (!indexes.ContainsKey(data[0]))
                {
                    indexes[data[0]] = new Index
                    {
                        IndexName = data[0],
                        IndexSymbol = data[0]
                    };
                }
                var stock = new Stock
                {
                    FullName = data[2],
                    Symbol = data[1],
                    Index = indexes[data[0]],
                };
                int lowerBound = int.Parse(data[3]);
                int upperBound = int.Parse(data[4]);

                _instance._priceRange.Add(new Tuple<Stock, Tuple<int, int>>(stock, Tuple.Create(lowerBound, upperBound)));
            }
            _instance._indexes.AddRange(indexes.Values);

            Trace.WriteLine(string.Format("Loaded {0} items", _instance._priceRange.Count));

            return _instance;
        }

        public IEnumerable<Index> AllIndexes
        {
            get { return _indexes; }
        }

        public IEnumerable<Stock> AllStocks
        {
            get { return _priceRange.Select(p => p.Item1); }
        }

        public IDisposable Start()
        {
            if (null != _thread)
                throw new InvalidOperationException("already started!");

            _thread = new Thread(Run);
            _thread.Start();
            return new ThreadRunner(this);
        }

        public event EventHandler<StockChangedEventArgs> StockChanged;

        private async void Run()
        {
            await Task.Delay(1000);
            foreach (var item in _priceRange)
            {
                RaiseEvent(item.Item1,  item.Item2.Item1 / 100.0);
            }
            while (true)
            {
                int nextIndex = _rnd.Next(0, _priceRange.Count);
                var item = _priceRange[nextIndex];
                double nextPrice = _rnd.Next(item.Item2.Item1, item.Item2.Item2) / 100.0;
                RaiseEvent(item.Item1, nextPrice);
                await Task.Delay(_rnd.Next(10, 5000));
            }
        }

        void RaiseEvent(Stock stock, double price)
        {
            Trace.WriteLine(string.Format("New ticker for {0} - price {1}", stock.Symbol, price));
            var local = StockChanged;
            if (null == local)
                return;

            local(this, new StockChangedEventArgs
            {
                CurrentValue = price,
                Product = stock,
                Time = DateTimeOffset.UtcNow,
            });
        }

        private class ThreadRunner : IDisposable
        {
            private readonly StockTicker _owner;

            public ThreadRunner(StockTicker owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                _owner._thread.Abort();
                _owner._thread = null;
            }
        }
    }
}
