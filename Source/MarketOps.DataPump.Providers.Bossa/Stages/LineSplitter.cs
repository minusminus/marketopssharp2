using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Splits line separated by comma.
/// </summary>
internal static class LineSplitter
{
    public static IEnumerable<string[]> Split(this IEnumerable<string> lines)
    {
        foreach (string line in lines)
            yield return SplitLine(line);
    }

    private static string[] SplitLine(in string line) => 
        line.Split(BossaDaily.FieldsSeparator);
}
