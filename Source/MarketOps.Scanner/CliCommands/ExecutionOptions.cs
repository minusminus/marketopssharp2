namespace MarketOps.Scanner.CliCommands;

/// <summary>
/// Execution options from commandline.
/// </summary>
internal class ExecutionOptions
{
    public bool ParsedCorrectly = false;
    public string ScannerName;
    public string StockNamesFilePath;
    public int NumberOfSignalsPerStock;
}
