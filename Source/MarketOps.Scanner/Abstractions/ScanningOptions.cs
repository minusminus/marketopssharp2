namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Scanning options from commandline.
/// </summary>
internal record ScanningOptions
{
    public bool ParsedCorrectly = false;
    public string ScannerName;
    public string StockNamesFilePath;
    public string ResultsPath;
    public int NumberOfSignalsPerStock;
    public string ScannerParametersJson;
}
