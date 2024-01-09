using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.Stages;
using MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Stages;

[TestFixture]
internal class DataIndexFinderTests
{
    private readonly PkoFundsDefs _pkoFundsDefs = PkoFundsDefsTools.Get();

    [Test]
    public void FindIndexes_StockExists_TsExists__ReturnsCorrectIndexes()
    {
        StockDefinitionShort stockDef = new(1, StockType.InvestmentFund, "PKO002", new DateTime(2023, 11, 21));

        var (resultFundIndex, resultTsIndex) = DataIndexFinder.FindIndexes(PkoFundsTestData.FundsData, _pkoFundsDefs, stockDef);

        resultFundIndex.ShouldBe(2);
        resultTsIndex.ShouldBe(3);
    }

    [Test]
    public void FindIndexes_StockExists_TsDoesNotExist__ReturnsNotFoundTs([Values(2022, 2024)] int yearOutOfRange)
    {
        StockDefinitionShort stockDef = new(1, StockType.InvestmentFund, "PKO002", new DateTime(yearOutOfRange, 11, 21));

        var (resultFundIndex, resultTsIndex) = DataIndexFinder.FindIndexes(PkoFundsTestData.FundsData, _pkoFundsDefs, stockDef);

        resultFundIndex.ShouldBe(2);
        resultTsIndex.ShouldBe(PkoCsvData.NotFoundDataIndex);
    }

    [Test]
    public void FindIndexes_StockDoesNotExist_TsExists__ReturnsNotFoundStock()
    {
        StockDefinitionShort stockDef = new(1, StockType.InvestmentFund, "NotExistingStock", new DateTime(2023, 11, 21));

        var (resultFundIndex, resultTsIndex) = DataIndexFinder.FindIndexes(PkoFundsTestData.FundsData, _pkoFundsDefs, stockDef);

        resultFundIndex.ShouldBe(PkoCsvData.NotFoundDataIndex);
        resultTsIndex.ShouldBe(3);
    }

    [Test]
    public void FindIndexes_StockAndTsDoNotExist__ReturnsNotFoundForBoth()
    {
        StockDefinitionShort stockDef = new(1, StockType.InvestmentFund, "NotExistingStock", new DateTime(2022, 11, 21));

        var (resultFundIndex, resultTsIndex) = DataIndexFinder.FindIndexes(PkoFundsTestData.FundsData, _pkoFundsDefs, stockDef);

        resultFundIndex.ShouldBe(PkoCsvData.NotFoundDataIndex);
        resultTsIndex.ShouldBe(PkoCsvData.NotFoundDataIndex);
    }
}
