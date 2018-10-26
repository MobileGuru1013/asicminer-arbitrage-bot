using System.Collections.Generic;

namespace ArbitrageBot
{
    internal class OrderBookData
    {
        public IBotSource Source { get; set; }

        public IEnumerable<OrderBookOrder> Bids { get; set; }

        public IEnumerable<OrderBookOrder> Asks { get; set; }
    }
}