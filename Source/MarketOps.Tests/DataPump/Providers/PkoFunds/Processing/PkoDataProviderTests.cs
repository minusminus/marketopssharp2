using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.Exceptions;
using MarketOps.DataPump.Providers.PkoFunds.Processing;
using MarketOps.DataPump.Common;
using MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Processing;

[TestFixture]
internal class PkoDataProviderTests
{
    private IPkoFundsDataBuffer _dataBuffer = null!;
    private readonly PkoFundsDefs _pkoFundsDefs = PkoFundsDefsTools.Get();
    private PkoDataProvider _testObj = null!;
    private ILogger<PkoDataProvider> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<PkoDataProvider>>();
        _dataBuffer = Substitute.For<IPkoFundsDataBuffer>();
        _dataBuffer.Get().Returns(PkoFundsTestData.FundsData);

        _testObj = new PkoDataProvider(_dataBuffer, _pkoFundsDefs, _logger);
    }

    [Test]
    public void Get_CorrectData__ReturnsCorrectly()
    {
        var def = new StockDefinitionShort(1, StockType.InvestmentFund, "PKO001", DateTime.MinValue);

        var result = _testObj.Get(PumpingDataRange.Daily, def).ToList();

        result.Count.ShouldBe(5);
    }

    [Test]
    public void Get_NotExistingFundName__Throws()
    {
        var def = new StockDefinitionShort(1, StockType.InvestmentFund, "NotExistingFund", DateTime.MinValue);

        Should.Throw<PkoFundsFundNotFoundException>(() => _testObj.Get(PumpingDataRange.Daily, def).ToList());
    }
}
