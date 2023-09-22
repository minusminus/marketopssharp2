using AutoFixture;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Stages;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.Bossa.Stages;

[TestFixture]
internal class PumpingDataBuilderTests
{
    private readonly StockDataRange stockDataRange = StockDataRange.Daily;
    private readonly StockDefinitionShort stockDefinitionShort = new(1, StockType.Stock, "test");
    private readonly Fixture fixture = new();

    [Test]
    public void ToPumpingData__ConvertsCorrectly([Values(1, 2)] int count)
    {
        var input = Enumerable.Range(1, count)
            .Select(_ => fixture.CreateMany<string>(7).ToArray())
            .ToList();

        var result = PumpingDataBuilder.ToPumpingData(input, stockDataRange, stockDefinitionShort).ToList();

        result.Count.ShouldBe(input.Count);
        for (int i = 0; i < input.Count; i++)
            CheckResultItem(result[i], input[i]);

        void CheckResultItem(PumpingData item, string[] expected)
        {
            item.DataRange.ShouldBe(stockDataRange);
            item.StockDefinition.ShouldBe(stockDefinitionShort);
            item.O.ShouldBe(expected[PumpingDataBuilder.IndexOpen]);
            item.H.ShouldBe(expected[PumpingDataBuilder.IndexHigh]);
            item.L.ShouldBe(expected[PumpingDataBuilder.IndexLow]);
            item.C.ShouldBe(expected[PumpingDataBuilder.IndexClose]);
            item.V.ShouldBe(expected[PumpingDataBuilder.IndexVolume]);
            item.Ts.ShouldBe(expected[PumpingDataBuilder.IndexDt]);
        }
    }
}
