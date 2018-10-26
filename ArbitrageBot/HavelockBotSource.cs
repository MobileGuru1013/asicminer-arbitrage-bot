using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace ArbitrageBot
{
    internal class HavelockBotSource : IBotSource
    {
        private static readonly string BaseOrdersUri = "https://www.havelockinvestments.com/r/";

        private readonly string _securityName;
        private readonly int _numberOfSharesPer;

        public HavelockBotSource(string securityName, int numberOfSharesPer)
        {
            if (securityName == null)
                throw new ArgumentNullException("securityName");

            _securityName = securityName;
            _numberOfSharesPer = numberOfSharesPer;
        }

        public string Name
        {
            get { return "Havelock:" + _securityName; }
        }

        public int NumberOfSharesPer
        {
            get { return _numberOfSharesPer; }
        }

        public decimal BuyingFee
        {
            get { return 0m; }
        }

        public decimal SellingFee
        {
            get { return 0.004m; }
        }

        public OrderBookData Retrieve()
        {
            var we = new WebClient();
            we.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1468.0 Safari/537.36");

            var nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("symbol", _securityName);

            var results = we.UploadValues(BaseOrdersUri + "/orderbook", nameValueCollection);
            var json = Encoding.ASCII.GetString(results);

            var data = new OrderBookData();
            var havelockOrderBook = JsonConvert.DeserializeObject<HavelockOrderBookJson>(json);

            data.Bids = ToOrders(havelockOrderBook.bids);
            data.Asks = ToOrders(havelockOrderBook.asks);
 
            data.Source = this;
            return data;
        }

        private static IEnumerable<OrderBookOrder> ToOrders(Dictionary<decimal, decimal> bids)
        {
            return bids.Select(b => new OrderBookOrder() { Amount = b.Key, Quantity = b.Value});
        }

        private class HavelockOrderBookJson
        {
            public string status { get; set; }

            public Dictionary<decimal, decimal> bids { get; set; }

            public Dictionary<decimal, decimal> asks { get; set; }
        }
    }
}