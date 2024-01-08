using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.Stages;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Stages;

[TestFixture]
internal class PumpingDataBuilderTests
{
    private readonly StockDefinitionShort _stockDef = new(1, StockType.InvestmentFund, "PKO002", new DateTime(2023, 11, 19));

    [Test]
    public void ToPumpingData__ReturnsCorrectly()
    {
        var input = new List<StageData>()
        {
            new StageData("2023-11-20", "1"),
            new StageData("2023-11-21", "2"),
            new StageData("2023-11-22", "3"),
        };

        var result = PumpingDataBuilder.ToPumpingData(input, PumpingDataRange.Daily, _stockDef).ToList();

        result.Count.ShouldBe(input.Count);
        CheckPumpingData(result[0], "20231120", "1");
        CheckPumpingData(result[1], "20231121", "2");
        CheckPumpingData(result[2], "20231122", "3");
    }

    [Test]
    public void ToPumpingData_EmptyInput__ReturnsEmpty()
    {
        PumpingDataBuilder.ToPumpingData(new List<StageData>(), PumpingDataRange.Daily, _stockDef).ShouldBeEmpty();
    }

    private void CheckPumpingData(PumpingData data, string expectedTs, string expectedPrice)
    {
        data.StockDefinition.ShouldBe(_stockDef);
        data.DataRange.ShouldBe(PumpingDataRange.Daily);
        data.O.ShouldBe(expectedPrice);
        data.H.ShouldBe(expectedPrice);
        data.L.ShouldBe(expectedPrice);
        data.C.ShouldBe(expectedPrice);
        data.V.ShouldBe("0");
        data.Ts.ShouldBe(expectedTs);
    }
}
