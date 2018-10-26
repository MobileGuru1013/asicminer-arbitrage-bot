using System;
using System.Collections.Generic;

namespace ArbitrageBot
{
    internal class ArbitrageResult
    {
        public decimal Profit
        {
            get { return Proceeds - Fee; }
        }

        public decimal Proceeds { get; set; }

        public decimal Fee { get; set; }

        public DateTime Timestamp { get; private set; }

        public IEnumerable<OrderBookOrder> BidsToFill { get; set; }

        public IEnumerable<OrderBookOrder> AsksToFill { get; set; }

        public string BidSource { get; set; }

        public string AskSource { get; set; }

        public decimal BidAmount { get; set; }

        public decimal AskAmount { get; set; }

        public ArbitrageResult()
        {
            Timestamp = DateTime.Now;
        }
    }
}