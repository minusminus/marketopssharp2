using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Creates PumpingData from splitted line.
/// </summary>
internal static class PumpingDataBuilder
{
    public static IEnumerable<PumpingData> ToPumpingData(this IEnumerable<string[]> lines, PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        foreach (var line in lines)
            yield return BuildPumpingData(line, dataRange, stockDefinition);
    }

    private static PumpingData BuildPumpingData(in string[] lines, in PumpingDataRange dataRange, in StockDefinitionShort stockDefinition) =>
        new(
            dataRange,
            stockDefinition,
            lines[BossaDailyIndex.Open],
            lines[BossaDailyIndex.High],
            lines[BossaDailyIndex.Low],
            lines[BossaDailyIndex.Close],
            IntegerPartOnly(lines[BossaDailyIndex.Volume]),
            lines[BossaDailyIndex.Dt]
        );

    private static string IntegerPartOnly(in string value)
    {
        int separatorIndex = value.IndexOf(BossaDaily.NumberSeparator);
        return (separatorIndex == -1)
            ? value
            : value[..separatorIndex];
    }
}
