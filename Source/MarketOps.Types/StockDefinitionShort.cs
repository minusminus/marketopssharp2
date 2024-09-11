namespace MarketOps.Types;

/// <summary>
/// Short definition of stock.
/// </summary>
public record StockDefinitionShort(
    int Id,
    StockType Type,
    string Name,
    DateTime LastTs);