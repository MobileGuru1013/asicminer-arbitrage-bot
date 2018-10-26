using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ArbitrageBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bots = new List<IBotSource>()
                {
                     new HavelockBotSource("ASICM", 100),
                     new BtctBotSource("TAT.ASICMINER", 100),
                     new BtctBotSource("ASICMINER-PT", 1)
                };

            while (true)
            {
                var all = new ConcurrentBag<OrderBookData>();
                try
                {
                    foreach (var bot in bots)
                    {
                        var result = bot.Retrieve();
                        all.Add(result);
                        Thread.Sleep(100);
                    }

                    // btct doesn't seem to like it if you make requests simultaneously...
                    //Parallel.ForEach(bots, b =>
                    //               {
                    //                   var result = b.Retrieve();
                    //                   all.Add(result);
                    //               });

                    var arbitrageResults = Arbiter.DetectOpportunities(all);
                    Log(arbitrageResults);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Thread.Sleep(5000);
            }
        }

        private static void Log(IEnumerable<ArbitrageResult> arbitrageResults)
        {
            const decimal minimumProfitIWant = 0.001m;
            foreach (var result in arbitrageResults)
            {
                if (result.Profit > minimumProfitIWant)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(result.Timestamp.ToShortTimeString() + ": Sell " + result.BidSource + " " + result.BidAmount + ", buy " + result.AskSource + " " + result.AskAmount);
                    Console.WriteLine("\tDifference: " + result.Proceeds);
                    Console.WriteLine("\tFees: " + result.Fee);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\tProfit: " + result.Profit);
                }
            }
        }
    }
}
