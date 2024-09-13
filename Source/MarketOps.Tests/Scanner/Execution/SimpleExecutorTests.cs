using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;
using MarketOps.Scanner.Execution;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.Tests.Scanner.Execution;

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

[TestFixture]
internal class SimpleExecutorTests
{
    private IStockNamesLoader _stockNamesLoader;
    private IScannerStockDataProvider _stockDataProvider;
    private IScanResultProcessor _resultProcessor;
    private IScannersFactory _scannerFactory;
    private IScanner _scanner;
    private ILogger<SimpleExecutor> _logger;

    private SimpleExecutor _testObj;

    [SetUp]
    public void SetUp()
    {
        _stockNamesLoader = Substitute.For<IStockNamesLoader>();
        _stockDataProvider = Substitute.For<IScannerStockDataProvider>();
        _resultProcessor = Substitute.For<IScanResultProcessor>();
        _scannerFactory = Substitute.For<IScannersFactory>();
        _scanner = Substitute.For<IScanner>();
        _logger = Substitute.For<ILogger<SimpleExecutor>>();

        _testObj = new SimpleExecutor(new ScanningOptions(), _stockNamesLoader, _stockDataProvider, _resultProcessor, _scannerFactory, _logger);
    }

    [Test]
    public async Task WithScanner_AndStocks__ProcessesAllStocks()
    {
        //given
        string[] stockNames = ["Stock1", "Stock2"];
        _scannerFactory.GetScanner(default).ReturnsForAnyArgs(_scanner);
        _stockNamesLoader.GetStockNames(default).ReturnsForAnyArgs(stockNames);
        _stockDataProvider.GetStockDefinitionAsync(default).ReturnsForAnyArgs(Task.FromResult(new StockDefinitionShort(1, StockType.Stock, string.Empty, DateTime.Now)));
        _stockDataProvider.GetStockDataAsync(default).ReturnsForAnyArgs(Task.FromResult(new StockData(1, [], [], [], [], [], [])));

        //when
        await _testObj.Execute(CancellationToken.None);

        //then
        _scannerFactory.Received(1).GetScanner(Arg.Any<string>());
        _scanner.Received(1).SetParameters(Arg.Any<string>());
        _stockNamesLoader.Received(1).GetStockNames(Arg.Any<string>());
        await _stockDataProvider.Received(stockNames.Length).GetStockDefinitionAsync(Arg.Any<string>());
        await _stockDataProvider.Received(stockNames.Length).GetStockDataAsync(Arg.Any<StockDefinitionShort>());
        _scanner.Received(stockNames.Length).Scan(Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
        _resultProcessor.Received(stockNames.Length).ProcessResult(Arg.Any<string>(), Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
    }

    [Test]
    public async Task WithScanner_NoStocks__DoesNoProcessing()
    {
        //given
        string[] stockNames = [];
        _scannerFactory.GetScanner(default).ReturnsForAnyArgs(_scanner);
        _stockNamesLoader.GetStockNames(default).ReturnsForAnyArgs(stockNames);
        _stockDataProvider.GetStockDefinitionAsync(default).ReturnsForAnyArgs(Task.FromResult(new StockDefinitionShort(1, StockType.Stock, string.Empty, DateTime.Now)));
        _stockDataProvider.GetStockDataAsync(default).ReturnsForAnyArgs(Task.FromResult(new StockData(1, [], [], [], [], [], [])));

        //when
        await _testObj.Execute(CancellationToken.None);

        //then
        _scannerFactory.Received(1).GetScanner(Arg.Any<string>());
        _scanner.Received(1).SetParameters(Arg.Any<string>());
        _stockNamesLoader.Received(1).GetStockNames(Arg.Any<string>());
        await _stockDataProvider.DidNotReceive().GetStockDefinitionAsync(Arg.Any<string>());
        await _stockDataProvider.DidNotReceive().GetStockDataAsync(Arg.Any<StockDefinitionShort>());
        _scanner.DidNotReceive().Scan(Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
        _resultProcessor.DidNotReceive().ProcessResult(Arg.Any<string>(), Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
    }

    [Test]
    public async Task ScannerNotFound__Throws()
    {
        //given
        IScanner nullScanner = null!;
        _scannerFactory.GetScanner(default).ReturnsForAnyArgs(nullScanner);

        //when
        await Should.ThrowAsync<Exception>(() => _testObj.Execute(CancellationToken.None));

        //then
        _scannerFactory.Received(1).GetScanner(Arg.Any<string>());
        _scanner.DidNotReceive().SetParameters(Arg.Any<string>());
        _stockNamesLoader.DidNotReceive().GetStockNames(Arg.Any<string>());
        await _stockDataProvider.DidNotReceive().GetStockDefinitionAsync(Arg.Any<string>());
        await _stockDataProvider.DidNotReceive().GetStockDataAsync(Arg.Any<StockDefinitionShort>());
        _scanner.DidNotReceive().Scan(Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
        _resultProcessor.DidNotReceive().ProcessResult(Arg.Any<string>(), Arg.Any<StockData>(), Arg.Any<ScanResult[]>());
    }
}
