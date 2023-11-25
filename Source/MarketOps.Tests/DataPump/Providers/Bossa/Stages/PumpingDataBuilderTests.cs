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
        var input = PrepareInputLines(count);

        var result = PumpingDataBuilder.ToPumpingData(input, dataRange, stockDefinitionShort).ToList();

        result.Count.ShouldBe(input.Count);
        for (int i = 0; i < input.Count; i++)
            CheckResultItem(result[i], input[i], input[i][BossaDailyIndex.Volume]);
    }

    [TestCase("123456", "123456")]
    [TestCase("123456.789", "123456")]
    public void ToPumpingData_VolumeConversion__ConvertsCorrectly(string volumeValue, string expectedValue)
    {
        var input = PrepareInputLines(1);
        input[0][BossaDailyIndex.Volume] = volumeValue;

        var result = PumpingDataBuilder.ToPumpingData(input, dataRange, stockDefinitionShort).ToList();

        result.Count.ShouldBe(1);
        CheckResultItem(result[0], input[0], expectedValue);
    }

    private List<string[]> PrepareInputLines(int count) => 
        Enumerable.Range(1, count)
            .Select(_ => fixture.CreateMany<string>(BossaDaily.StandardFieldsInLine).ToArray())
            .ToList();

    private void CheckResultItem(PumpingData item, string[] expected, string expectedVolume)
    {
        item.DataRange.ShouldBe(dataRange);
        item.StockDefinition.ShouldBe(stockDefinitionShort);
        item.O.ShouldBe(expected[BossaDailyIndex.Open]);
        item.H.ShouldBe(expected[BossaDailyIndex.High]);
        item.L.ShouldBe(expected[BossaDailyIndex.Low]);
        item.C.ShouldBe(expected[BossaDailyIndex.Close]);
        item.V.ShouldBe(expectedVolume);
        item.Ts.ShouldBe(expected[BossaDailyIndex.Dt]);
    }
}
