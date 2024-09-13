using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;
using Microsoft.Extensions.Logging;

namespace MarketOps.Scanner.Execution;

/// <summary>
/// Executes scanning.
/// </summary>
internal class SimpleExecutor : IScanningExecutor
{
    private const string LogHeaderOptions = "Scanner options";
    private const string LogHeaderScanner = "Scanner";
    private const string LogHeaderStock = "Stock";

    private readonly ScanningOptions _scanningOptions;
    private readonly IStockNamesLoader _stockNamesLoader;
    private readonly IScannerStockDataProvider _stockDataProvider;
    private readonly IScanResultProcessor _resultProcessor;
    private readonly IScannersFactory _scannerFactory;
    private readonly ILogger<SimpleExecutor> _logger;

    public SimpleExecutor(ScanningOptions scanningOptions, IStockNamesLoader stockNamesLoader, IScannerStockDataProvider stockDataProvider,
        IScanResultProcessor resultProcessor, IScannersFactory scannerFactory, ILogger<SimpleExecutor> logger)
    {
        _scanningOptions = scanningOptions;
        _stockNamesLoader = stockNamesLoader;
        _stockDataProvider = stockDataProvider;
        _resultProcessor = resultProcessor;
        _scannerFactory = scannerFactory;
        _logger = logger;
    }

    public async Task Execute(CancellationToken token)
    {
        LogScanningOptions();

        IScanner scanner = CreateAndInitializeScanner();
        string[] stockNames = GetStockNames();
        for (int i = 0; i < stockNames.Length; i++)
        {
            if (token.IsCancellationRequested) return;
            _logger.LogInformation("[{Header}] {StockName}", LogHeaderStock, stockNames[i]);
            await ProcessStock(stockNames[i], _scanningOptions.NumberOfSignalsPerStock, scanner);
        }
    }

    private void LogScanningOptions()
    {
        _logger.LogInformation("[{Header}] Selected scanner: {ScannerName}", LogHeaderOptions, _scanningOptions.ScannerName);
        _logger.LogInformation("[{Header}] Results path: {ResultsPath}", LogHeaderOptions, _scanningOptions.ResultsPath);
        _logger.LogInformation("[{Header}] Number of signals per stock: {NumberOfSignalsPerStock}", LogHeaderOptions, _scanningOptions.NumberOfSignalsPerStock);
        _logger.LogInformation("[{Header}] Scanner parameters: {ScannerParameters}", LogHeaderOptions, _scanningOptions.ScannerParametersJson);
    }

    private IScanner CreateAndInitializeScanner()
    {
        IScanner scanner = _scannerFactory.GetScanner(_scanningOptions.ScannerName)
            ?? throw new Exception($"Scanner {_scanningOptions.ScannerName} not found.");
        if (!string.IsNullOrWhiteSpace(_scanningOptions.ScannerParametersJson))
            scanner.SetParameters(_scanningOptions.ScannerParametersJson);
        _logger.LogInformation("[{Header}] Current parameters: {CurrentParameters}", LogHeaderScanner, scanner.GetCurrentParameters());

        return scanner;
    }

    private string[] GetStockNames() => 
        _stockNamesLoader.GetStockNames(_scanningOptions.StockNamesFilePath);

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
