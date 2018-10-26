using System.Collections.Generic;

namespace ArbitrageBot
{
    internal class OrderFillInfo
    {
        public List<OrderBookOrder> Orders { get; private set; }

        public decimal Amount { get; set; }

        public OrderFillInfo()
        {
            Orders = new List<OrderBookOrder>();
        }
    }
}