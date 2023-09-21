namespace MarketOps.Types;

/// <summary>
/// Types of stocks.
/// </summary>
public enum StockType
{
    Undefined = -1,
    Stock = 0,
    Index = 1,
    IndexFuture = 2,
    InvestmentFund = 4,
    NBPCurrency = 5,
    Forex = 6
}
