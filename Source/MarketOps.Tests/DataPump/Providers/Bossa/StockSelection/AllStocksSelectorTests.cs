using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.StockSelection;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.Bossa.StockSelection;

[TestFixture]
internal class AllStocksSelectorTests
{
    private IDataPumpStocksDataProvider _stocksDataProvider = null!;
    private AllStocksSelector _testObj = null!;

    private readonly List<StockDefinitionShort> _stockDefinitions = new()
    {
        new(1, StockType.Undefined, "test", DateTime.MinValue)
    };

    [SetUp]
    public void SetUp()
    {
        _stocksDataProvider = Substitute.For<IDataPumpStocksDataProvider>();
        _stocksDataProvider.GetAllActive(default).ReturnsForAnyArgs(_stockDefinitions);

        _testObj = new AllStocksSelector(_stocksDataProvider);
    }

    [Test]
    public void SelectStocks__ReturnsCorrectly([Values] StockType stockType)
    {
        var result = _testObj.SelectStocks(stockType).ToList();

        result.ShouldBe(_stockDefinitions);
        _stocksDataProvider.DidNotReceiveWithAnyArgs().GetAllActive(default, default!);
        _stocksDataProvider.ReceivedWithAnyArgs(1).GetAllActive(stockType);
    }
}
