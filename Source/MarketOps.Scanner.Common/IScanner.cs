namespace MarketOps.Scanner.Common;

/// <summary>
/// Interface for scanning mechanism.
/// </summary>
public interface IScanner
{
    public ScanResult[] Scan(StockData data, int numberOfSignals);
}
