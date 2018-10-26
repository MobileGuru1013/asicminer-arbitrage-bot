using System.Collections.Generic;
using System.Linq;

namespace ArbitrageBot
{
    internal class Arbiter
    {
        /// <summary>
        /// Detect arbitrage opportunities between the specified order books
        /// </summary>
        /// <param name="orderBooks"></param>
        /// <returns>A list of all arbitrage opportunities</returns>
        public static IEnumerable<ArbitrageResult> DetectOpportunities(IEnumerable<OrderBookData> orderBooks)
        {
            var results = new List<ArbitrageResult>();
            foreach (var bidData in orderBooks)
            {
                foreach (var askData in orderBooks.Where(x => x != bidData))
                {
                    var result = new ArbitrageResult();

                    var asks = askData.Asks.OrderBy(o => o.Amount);
                    var bids = bidData.Bids.OrderByDescending(o => o.Amount);
                    var askFill = AmountForQuantity(askData.Source, asks);
                    var bidFill = AmountForQuantity(bidData.Source, bids);

                    result.Proceeds = bidFill.Amount - askFill.Amount;
                    result.Fee = bidFill.Amount * bidData.Source.SellingFee + askFill.Amount * askData.Source.BuyingFee;
                    result.BidSource = bidData.Source.Name;
                    result.AskSource = askData.Source.Name;
                    result.BidsToFill = bidFill.Orders;
                    result.AsksToFill = askFill.Orders;
                    result.BidAmount = bidFill.Amount;
                    result.AskAmount = askFill.Amount;
                    if(result.Profit > 0)
                        results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Determine the quantity and cost of filling a "single" share worth
        /// (i.e. 1 share of ASICMINER or 100 shares of a 1/100th PT)
        /// </summary>
        /// <param name="source">Where is the data coming from </param>
        /// <param name="data">The Order book data</param>
        /// <returns>Order fill info</returns>
        private static OrderFillInfo AmountForQuantity(IBotSource source, IEnumerable<OrderBookOrder> data)
        {
            var fillInfo = new OrderFillInfo();
            decimal remainingQuantity = source.NumberOfSharesPer;
            foreach (var order in data)
            {
                if (order.Quantity >= remainingQuantity)
                {
                    fillInfo.Amount += (remainingQuantity) * order.Amount;
                    fillInfo.Orders.Add(order);
                    break;
                }
                else
                {
                    remainingQuantity -= order.Quantity;
                    fillInfo.Orders.Add(order);
                    fillInfo.Amount += order.Quantity * order.Amount;
                }
            }

            return fillInfo;
        }
    }
}