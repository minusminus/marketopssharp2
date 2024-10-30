namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Scanning options from commandline.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
internal record ScanningOptions
{
    public bool ParsedCorrectly = false;
    public string ScannerName;
    public string StockNamesFilePath;
    public string ResultsPath;
    public int NumberOfSignalsPerStock;
    public string ScannerParametersJson;
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
