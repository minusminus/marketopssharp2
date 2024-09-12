using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;

namespace MarketOps.Scanner.Execution;

/// <summary>
/// Executes scanning.
/// </summary>
internal class SimpleExecutor : IScanningExecutor
{
    private readonly ScanningOptions _scanningOptions;
    private readonly IStockNamesLoader _stockNamesLoader;
    private readonly IScannerStockDataProvider _stockDataProvider;
    private readonly IScanResultProcessor _resultProcessor;
    private readonly IScannersFactory _scannerFactory;

    public SimpleExecutor(ScanningOptions scanningOptions, IStockNamesLoader stockNamesLoader, IScannerStockDataProvider stockDataProvider,
        IScanResultProcessor resultProcessor, IScannersFactory scannerFactory)
    {
        _scanningOptions = scanningOptions;
        _stockNamesLoader = stockNamesLoader;
        _stockDataProvider = stockDataProvider;
        _resultProcessor = resultProcessor;
        _scannerFactory = scannerFactory;
    }

    public async Task Execute(CancellationToken token)
    {
        var scanner = _scannerFactory.GetScanner(_scanningOptions.ScannerName);
        var stockNames = _stockNamesLoader.GetStockNames(_scanningOptions.StockNamesFilePath);
        for (int i = 0; i < stockNames.Length; i++)
        {
            if (token.IsCancellationRequested) return;
            await ProcessStock(stockNames[i], _scanningOptions.NumberOfSignalsPerStock, scanner);
        }
    }

    private async Task ProcessStock(string stockName, int numberOfSignals, IScanner scanner)
    {
        var stockDefinition = await _stockDataProvider.GetStockDefinitionAsync(stockName);
        if (stockDefinition is null) return;
        var stockData = await _stockDataProvider.GetStockDataAsync(stockDefinition);
        if (stockData is null) return;
        ScanStockData(stockName, stockData, numberOfSignals, scanner);
    }

    private void ScanStockData(string stockName, StockData stockData, int numberOfSignals, IScanner scanner)
    {
        var signals = ScanResult.Initialize(numberOfSignals);
        scanner.Scan(stockData, signals);
        _resultProcessor.ProcessResult(stockName, stockData, signals);
    }
}
