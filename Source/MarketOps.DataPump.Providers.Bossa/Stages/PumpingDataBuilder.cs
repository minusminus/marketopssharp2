using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Creates PumpingData from splitted line.
/// </summary>
internal static class PumpingDataBuilder
{
    internal const int IndexTicker = 0;
    internal const int IndexDt = 1;
    internal const int IndexOpen = 2;
    internal const int IndexHigh = 3;
    internal const int IndexLow = 4;
    internal const int IndexClose = 5;
    internal const int IndexVolume = 6;

    public static IEnumerable<PumpingData> ToPumpingData(this IEnumerable<string[]> lines, StockDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        foreach (var line in lines)
            yield return BuildPumpingData(line, dataRange, stockDefinition);
    }

    private static PumpingData BuildPumpingData(in string[] lines, in StockDataRange dataRange, in StockDefinitionShort stockDefinition) =>
        new(
            dataRange,
            stockDefinition,
            lines[IndexOpen],
            lines[IndexHigh],
            lines[IndexLow],
            lines[IndexClose],
            lines[IndexVolume],
            lines[IndexDt]
        );
}
