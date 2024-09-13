using MarketOps.Scanner.Common;

namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Interface for processing scan results.
/// </summary>
internal interface IScanResultProcessor
{
    public void ProcessResult(string stockName, StockData stockData, in ScanResult[] result);
}
