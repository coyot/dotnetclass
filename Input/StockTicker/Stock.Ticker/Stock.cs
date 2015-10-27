namespace Stock.Ticker
{
    public class Stock
    {
        public string Symbol { get; internal set; }

        public string FullName { get; internal set; }

        public Index Index { get; internal set; }
    }
}