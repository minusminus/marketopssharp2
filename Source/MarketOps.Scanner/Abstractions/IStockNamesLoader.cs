namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Interface loading names of stocks from specified file.
/// </summary>
internal interface IStockNamesLoader
{
    public string[] GetStockNames(string filePath);
}
