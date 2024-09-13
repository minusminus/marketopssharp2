using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;

namespace MarketOps.Scanner.ResultProcessing;

/// <summary>
/// Result processor creating single file with signals for all stocks.
/// </summary>
internal class SingleFileResultProcessor : IScanResultProcessor
{
    private readonly string _filePath;

    public SingleFileResultProcessor(ScanningOptions scanningOptions)
    {
        var resultsDir = string.IsNullOrEmpty(scanningOptions.ResultsPath)
            ? Path.Combine(Consts.ExecutingLocation, "ScanningResults")
            : scanningOptions.ResultsPath;
        Directory.CreateDirectory(resultsDir);
        _filePath = Path.Combine(resultsDir, $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.scanresult");
    }

    public void ProcessResult(string stockName, StockData stockData, in ScanResult[] result)
    {
        using var file = File.Open(_filePath, FileMode.OpenOrCreate);
        file.Seek(0, SeekOrigin.End);
        using var stream = new StreamWriter(file);
        WriteResult(stream, stockName, stockData, result);
    }

    private void WriteResult(StreamWriter file, string stockName, StockData stockData, in ScanResult[] result)
    {
        for (int i = 0; i < result.Length; i++)
            file.WriteLine($"{stockName},{stockData.Ts[i].ToString("yyyy-MM-dd")},{result[i].Signal}");
    }
}
