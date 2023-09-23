using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Creates PumpingData from splitted line.
/// </summary>
internal static class PumpingDataBuilder
{
    public static IEnumerable<PumpingData> ToPumpingData(this IEnumerable<string[]> lines, StockDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        foreach (var line in lines)
            yield return BuildPumpingData(line, dataRange, stockDefinition);
    }

    private static PumpingData BuildPumpingData(in string[] lines, in StockDataRange dataRange, in StockDefinitionShort stockDefinition) =>
        new(
            dataRange,
            stockDefinition,
            lines[BossaDailyIndex.Open],
            lines[BossaDailyIndex.High],
            lines[BossaDailyIndex.Low],
            lines[BossaDailyIndex.Close],
            lines[BossaDailyIndex.Volume],
            lines[BossaDailyIndex.Dt]
        );
}
