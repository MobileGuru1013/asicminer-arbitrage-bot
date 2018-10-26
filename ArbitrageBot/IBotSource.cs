namespace ArbitrageBot
{
    internal interface IBotSource
    {
        OrderBookData Retrieve();

        string Name { get; }

        int NumberOfSharesPer { get; }

        decimal BuyingFee { get; }

        decimal SellingFee { get; }
    }
}