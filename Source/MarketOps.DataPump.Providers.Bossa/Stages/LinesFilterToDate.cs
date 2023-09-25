using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Filters splitted data lines up to specified datetime.
/// To be executed after splitting line to cells.
/// Splitted lines should be verified and correct.
/// </summary>
internal static class LinesFilterToDate
{
    public static IEnumerable<string[]> FilterOutToDate(this IEnumerable<string[]> lines, DateTime ts, PumpingDataRange dataRange) =>
        dataRange switch
        {
            PumpingDataRange.Daily => FilterOutDaily(lines, ts),
            _ => throw new ArgumentException($"Not supported data range {dataRange}", nameof(dataRange)),
        };

    private static IEnumerable<string[]> FilterOutDaily(IEnumerable<string[]> lines, DateTime ts) =>
        FilterOut(lines, ts.ToString("yyyyMMdd"));

    private static IEnumerable<string[]> FilterOut(IEnumerable<string[]> lines, string ts)
    {
        foreach (var line in lines)
            if (LineTsGreaterThan(line[BossaDailyIndex.Dt], ts))
                yield return line;
    }

    private static bool LineTsGreaterThan(in string lineTs, in string ts) =>
        lineTs.CompareTo(ts) > 0;
}
