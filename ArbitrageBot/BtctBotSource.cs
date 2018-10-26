using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ArbitrageBot
{
    internal class BtctBotSource : IBotSource
    {
        private static readonly string BaseOrdersUri = "https://btct.co/api/orders/";

        private readonly string _securityName;
        private readonly int _numberOfSharesPer;

        public BtctBotSource(string securityName, int numberOfSharesPer)
        {
            if (securityName == null)
                throw new ArgumentNullException("securityName");

            _securityName = securityName;
            _numberOfSharesPer = numberOfSharesPer;
        }

        public string Name
        {
            get { return "Btct:" +_securityName; }
        }

        public int NumberOfSharesPer
        {
            get { return _numberOfSharesPer; }
        }

        public decimal BuyingFee
        {
            get { return 0.002m; }
        }

        public decimal SellingFee
        {
            get { return 0.002m; }
        }

        public OrderBookData Retrieve()
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1468.0 Safari/537.36");
                string results = wc.DownloadString(new Uri(BaseOrdersUri + _securityName));
           
                results = Regex.Replace(results, ",\"generated\":.*}", "}");
                var orderBookData = new OrderBookData();
               
                var orderBook = JsonConvert.DeserializeObject<Dictionary<string, BtctOrderBookJson>>(results).Select(s => s.Value);
                orderBookData.Asks = orderBook.Where(o => o.buy_sell == "ask").Select(o => new OrderBookOrder {Quantity = o.quantity, Amount = o.amount});
                orderBookData.Bids = orderBook.Where(o => o.buy_sell == "bid").Select(o => new OrderBookOrder { Quantity = o.quantity, Amount = o.amount });
                orderBookData.Source = this;

                return orderBookData;
            }
        }

        private class BtctOrderBookJson
        {
            public string ticker { get; set; }

            public long timestamp { get; set; }

            public decimal amount { get; set; }

            public decimal quantity { get; set; }

            public string buy_sell { get; set; }
        }
    }
}