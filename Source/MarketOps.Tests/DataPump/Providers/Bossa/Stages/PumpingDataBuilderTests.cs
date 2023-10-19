using AutoFixture;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Common;
using MarketOps.DataPump.Providers.Bossa.Stages;
using MarketOps.Tests.Autofixture;
using MarketOps.Types;

namespace MarketOps.Tests.DataPump.Providers.Bossa.Stages;

[TestFixture]
internal class PumpingDataBuilderTests
{
    private readonly PumpingDataRange dataRange = PumpingDataRange.Daily;
    private readonly StockDefinitionShort stockDefinitionShort = new(1, StockType.Stock, "test", DateTime.Now);
    private readonly IFixture fixture = FixtureFactory.Get();

    [Test]
    public void ToPumpingData__ConvertsCorrectly([Values(1, 2)] int count)
    {
        var input = Enumerable.Range(1, count)
            .Select(_ => fixture.CreateMany<string>(7).ToArray())
            .ToList();

        var result = PumpingDataBuilder.ToPumpingData(input, dataRange, stockDefinitionShort).ToList();

        result.Count.ShouldBe(input.Count);
        for (int i = 0; i < input.Count; i++)
            CheckResultItem(result[i], input[i]);

        void CheckResultItem(PumpingData item, string[] expected)
        {
            item.DataRange.ShouldBe(dataRange);
            item.StockDefinition.ShouldBe(stockDefinitionShort);
            item.O.ShouldBe(expected[BossaDailyIndex.Open]);
            item.H.ShouldBe(expected[BossaDailyIndex.High]);
            item.L.ShouldBe(expected[BossaDailyIndex.Low]);
            item.C.ShouldBe(expected[BossaDailyIndex.Close]);
            item.V.ShouldBe(expected[BossaDailyIndex.Volume]);
            item.Ts.ShouldBe(expected[BossaDailyIndex.Dt]);
        }
    }
}
