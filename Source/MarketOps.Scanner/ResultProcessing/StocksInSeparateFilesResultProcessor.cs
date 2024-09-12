using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;

namespace MarketOps.Scanner.ResultProcessing;

/// <summary>
/// Result processor creating separate file with signals for each stock.
/// </summary>
internal class StocksInSeparateFilesResultProcessor : IScanResultProcessor
{
    private readonly string _resultsDir;

    public StocksInSeparateFilesResultProcessor(ScanningOptions scanningOptions)
    {
        _resultsDir = string.IsNullOrEmpty(scanningOptions.ResultsPath)
            ? Path.Combine(Consts.ExecutingLocation, "ScanningResults")
            : scanningOptions.ResultsPath;
        Directory.CreateDirectory(_resultsDir);
    }

    public void ProcessResult(string stockName, StockData stockData, in ScanResult[] result)
    {
        var filePath = Path.Combine(_resultsDir, stockName);
        using var file = File.CreateText(filePath);
        WriteResult(file, stockData, result);
    }

    private void WriteResult(StreamWriter file, StockData stockData, in ScanResult[] result)
    {
        for (int i = 0; i < result.Length; i++)
            file.WriteLine($"{stockData.Ts[i].ToString("yyyy-MM-dd")},{result[i].Signal}");
    }
}
