namespace MarketOps.Types;

/// <summary>
/// Short definition of stock.
/// </summary>
public class StockDefinitionShort
{
    public readonly int Id;
    public readonly StockType Type;
    public readonly string Name;

    public StockDefinitionShort(int id, StockType type, string name)
    {
        Id = id;
        Type = type;
        Name = name;
    }

}
