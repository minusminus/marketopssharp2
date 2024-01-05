using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;
using MarketOps.DataPump.Providers.PkoFunds.Processing;
using MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Processing;

[TestFixture]
internal class PkoDataProviderTests
{
    private IPkoFundsDataBuffer _dataBuffer = null!;
    private readonly PkoFundsDefs _pkoFundsDefs = PkoFundsDefsTools.Get();
    private PkoDataProvider _testObj = null!;

    private readonly List<string> _testData = new()
    {
        "Data;PKO Zrównoważony;PKO Dynamicznej Alokacji;PKO Obligacji Skarbowych Krótkoterminowy",
        "2023-11-24;2310,07;112,23;106,99;",
        "2023-11-23;2309,50;112,21;106,98;",
        "2023-11-22;2309,40;112,19;106,96;",
        "2023-11-21;2310,00;112,16;106,94;",
        "2023-11-20;2309,91;112,14;106,92;",
    };

    [SetUp]
    public void SetUp()
    {        
        _dataBuffer = Substitute.For<IPkoFundsDataBuffer>();
        _dataBuffer.Get().Returns(PkoDataStreamReader.Read(DataStreamTools.CreateDataStream(_testData)));

        _testObj = new PkoDataProvider(_dataBuffer, _pkoFundsDefs);
    }

    [Test]
    public void Get_FirstReadOfStock__ReturnsCorrectData()
    {
        var def = new StockDefinitionShort(1, StockType.InvestmentFund, "PKO001", DateTime.MinValue);

        var result = _testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Daily, def).ToList();

        result.Count.ShouldBe(5);
    }
}
