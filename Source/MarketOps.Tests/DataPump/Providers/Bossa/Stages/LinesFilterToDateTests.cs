using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;
using MarketOps.DataPump.Providers.Bossa.Stages;

namespace MarketOps.Tests.DataPump.Providers.Bossa.Stages;

[TestFixture]
internal class LinesFilterToDateTests
{
    private readonly List<string[]> testDataDaily = new()
    {
        new string[] {"ADATEX", "20221201", "0.2980", "0.3280", "0.2980", "0.3000", "24000"},
        new string[] {"ADATEX", "20221202", "0.3000", "0.3020", "0.3000", "0.3000", "78665"},
        new string[] {"ADATEX", "20221205", "0.3240", "0.3800", "0.3000", "0.3000", "326891"},
        new string[] {"ADATEX", "20221206", "0.3000", "0.3000", "0.2920", "0.2920", "30215"},
    };

    [TestCase("2022-11-30", new string[] { "20221201", "20221202", "20221205", "20221206" })]
    [TestCase("2022-12-01", new string[] { "20221202", "20221205", "20221206" })]
    [TestCase("2022-12-02", new string[] { "20221205", "20221206" })]
    [TestCase("2022-12-06", new string[] { })]
    [TestCase("2022-12-07", new string[] { })]
    public void FilterOutToDate_DailyRange__ReturnsCorrectly(string ts, string[] expected)
    {
        int[] dateParts = SplitToDateParts(ts);
        var dt = new DateTime(dateParts[0], dateParts[1], dateParts[2]);

        var result = LinesFilterToDate.FilterOutToDate(testDataDaily, dt, PumpingDataRange.Daily).ToList();

        result.Count.ShouldBe(expected.Length);
        for (int i = 0; i < result.Count; i++)
            result[i][BossaDailyIndex.Dt].ShouldBe(expected[i]);

        static int[] SplitToDateParts(string ts) =>
            ts.Split('-')
            .Select(s => int.Parse(s))
            .ToArray();
    }

    [Test]
    public void FilterOutToDate_TickRange__Throws()
    {
        var dt = new DateTime(2022, 12, 02, 13, 20, 00);

        Should.Throw<ArgumentException>(() => LinesFilterToDate.FilterOutToDate(testDataDaily, dt, PumpingDataRange.Tick).ToList());
    }
}
