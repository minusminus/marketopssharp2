using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.StockSelection;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.StockSelection;

[TestFixture]
internal class OnlyPkoFundsSelectorTests
{
    private IDataPumpStocksDataProvider _stocksDataProvider = null!;
    private OnlyPkoFundsSelector _testObj = null!;

    private readonly List<StockDefinitionShort> _stockDefinitions = new()
    {
        new(1, StockType.Undefined, "test", DateTime.MinValue)
    };

    [SetUp]
    public void SetUp()
    {
        _stocksDataProvider = Substitute.For<IDataPumpStocksDataProvider>();
        _stocksDataProvider.GetAllActive(default, default!).ReturnsForAnyArgs(_stockDefinitions);

        _testObj = new OnlyPkoFundsSelector(_stocksDataProvider);
    }

    [Test]
    public void SelectStocks__ReturnsCorrectly([Values] StockType stockType)
    {
        var result = _testObj.SelectStocks(stockType).ToList();

        if(stockType == StockType.InvestmentFund)
        {
            result.ShouldBe(_stockDefinitions);
            _stocksDataProvider.ReceivedWithAnyArgs(1).GetAllActive(StockType.InvestmentFund, default!);
            _stocksDataProvider.DidNotReceiveWithAnyArgs().GetAllActive(default);
        }
        else
        {
            result.ShouldBeEmpty();
            _stocksDataProvider.DidNotReceiveWithAnyArgs().GetAllActive(default, default!);
            _stocksDataProvider.DidNotReceiveWithAnyArgs().GetAllActive(default);
        }
    }
}
