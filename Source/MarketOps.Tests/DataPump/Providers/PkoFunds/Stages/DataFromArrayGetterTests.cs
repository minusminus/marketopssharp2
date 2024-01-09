using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.Stages;
using MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Stages;

[TestFixture]
internal class DataFromArrayGetterTests
{
    [Test]
    public void GetSingleFundDataFromLastTs_FromBeginning__ReturnsAllData()
    {
        var result = DataFromArrayGetter.GetSingleFundDataFromLastTs(PkoFundsTestData.FundsData.Data, 1, PkoCsvData.NotFoundDataIndex).ToList();

        result.Count.ShouldBe(PkoFundsTestData.FundsData.Data.Length);
    }

    [Test]
    public void GetSingleFundDataFromLastTs_FromEndOfData__ReturnsEmpty()
    {
        DataFromArrayGetter.GetSingleFundDataFromLastTs(PkoFundsTestData.FundsData.Data, 1, 0).ShouldBeEmpty();
    }

    [Test]
    public void GetSingleFundDataFromLastTs_FromMidOfData__ReturnsOnlyRequiredData()
    {
        int lastTsIndex = PkoFundsTestData.FundsData.Data.Length / 2 + 1;
        var result = DataFromArrayGetter.GetSingleFundDataFromLastTs(PkoFundsTestData.FundsData.Data, 1, lastTsIndex).ToList();

        result.Count.ShouldBe(lastTsIndex);
    }
}
