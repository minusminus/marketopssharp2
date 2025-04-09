namespace MarketOps.Viewer.Backend.Entities;

internal enum StockType
{
    Stock = 0,
    Index = 1,
    IndexFuture = 2,
    // InvestmentFund = 4, // Pomijamy na razie
    // NBPCurrency = 5,    // Pomijamy na razie
    // Forex = 6           // Pomijamy na razie
    Undefined = -1 // Chociaż nie ma w bazie, może się przydać
}