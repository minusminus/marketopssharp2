namespace MarketOps.Scanner.Common;

/// <summary>
/// Interface for scanning mechanism.
/// 
/// Signals returned in order of data.
/// </summary>
public interface IScanner
{
    public void Scan(StockData data, in ScanResult[] result);
}
