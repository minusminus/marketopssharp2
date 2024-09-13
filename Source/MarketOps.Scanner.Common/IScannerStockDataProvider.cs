using MarketOps.Types;

namespace MarketOps.Scanner.Common;

/// <summary>
/// Interface providing stocks data for scanning.
/// 
/// StockData is ordered by TS descending (most current data first).
/// </summary>
public interface IScannerStockDataProvider
{
    public Task<StockDefinitionShort?> GetStockDefinitionAsync(string stockName);
    public Task<StockData> GetStockDataAsync(StockDefinitionShort stockDefinition);
}
