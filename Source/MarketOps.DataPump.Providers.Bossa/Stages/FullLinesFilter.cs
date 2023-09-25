using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Filters original data lines up to specified datetime.
/// To be executed before splitting line to cells.
/// </summary>
internal static class FullLinesFilter
{
    public static IEnumerable<string> FilterOutToDate(this IEnumerable<string> lines, DateTime ts, PumpingDataRange dataRange) =>
        dataRange switch
        {
            PumpingDataRange.Daily => FilterOutDaily(lines, ts),
            _ => throw new ArgumentException($"Not supported data range {dataRange}", nameof(dataRange)),
        };

    private static IEnumerable<string> FilterOutDaily(IEnumerable<string> lines, DateTime ts) => 
        FilterOut(lines, ts.ToString("yyyyMMdd"));

    private static IEnumerable<string> FilterOut(IEnumerable<string> lines, string ts)
    {
        int? firstSeparatorIndex = null;
        foreach (var line in lines)
        {
            if (!firstSeparatorIndex.HasValue)
                firstSeparatorIndex = FindLineFirstSeparator(line);

            if (LineAfterTs(line, ts, firstSeparatorIndex.Value))
                    yield return line;
        }
    }

    private static int FindLineFirstSeparator(in string line) => 
        line.IndexOf(BossaDaily.FieldsSeparator);

    private static bool LineAfterTs(in string line, in string ts, int firstSeparatorIndex) =>
        string.Compare(line, firstSeparatorIndex + 1, ts, 0, ts.Length) > 0;
}
