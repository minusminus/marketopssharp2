namespace MarketOps.Scanner.Common;

/// <summary>
/// Interface for scanning mechanism.
/// 
/// Signals are returned in order of data.
/// </summary>
public interface IScanner
{
    public void SetParameters(string parametersJson);
    public string GetCurrentParameters();

    public void Scan(StockData data, in ScanResult[] result);
}
