using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataFileDescriptions;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.Bossa.Stages;

/// <summary>
/// Verifies splitted line.
/// For incorrect line logs error and filters the line out.
/// </summary>
internal static class LinesVerifier
{
    public static IEnumerable<string[]> Verify(this IEnumerable<string[]> lines, PumpingDataRange dataRange, ILogger logger) =>
        dataRange switch
        {
            PumpingDataRange.Daily => VerifyDaily(lines, logger),
            _ => throw new ArgumentException($"Not supported data range {dataRange}", nameof(dataRange)),
        };

    private static IEnumerable<string[]> VerifyDaily(IEnumerable<string[]> lines, ILogger logger)
    {
        foreach (var line in lines)
            if (IsDailyLineCorrect(line))
                yield return line;
            else
                LogLineError(line, logger);
    }

    private static bool IsDailyLineCorrect(in string[] line)
    {
        if (line.Length != BossaDaily.FieldsInLine) return false;
        if ((line[BossaDailyIndex.Dt].Length != BossaDaily.DtLength) || (!OnlyDigits(line[BossaDailyIndex.Dt]))) return false;
        if (string.IsNullOrWhiteSpace(line[BossaDailyIndex.Open]) || !DigitsWithSingleSeparator(line[BossaDailyIndex.Open], BossaDaily.NumberSeparator)) return false;
        if (string.IsNullOrWhiteSpace(line[BossaDailyIndex.High]) || !DigitsWithSingleSeparator(line[BossaDailyIndex.High], BossaDaily.NumberSeparator)) return false;
        if (string.IsNullOrWhiteSpace(line[BossaDailyIndex.Low]) || !DigitsWithSingleSeparator(line[BossaDailyIndex.Low], BossaDaily.NumberSeparator)) return false;
        if (string.IsNullOrWhiteSpace(line[BossaDailyIndex.Close]) || !DigitsWithSingleSeparator(line[BossaDailyIndex.Close], BossaDaily.NumberSeparator)) return false;
        if (string.IsNullOrWhiteSpace(line[BossaDailyIndex.Volume]) || !OnlyDigits(line[BossaDailyIndex.Volume])) return false;
        return true;
    }

    private static void LogLineError(in string[] line, ILogger logger) => 
        logger.LogError("Incorrect data line: {Line}", string.Join(',', line));

    private static bool OnlyDigits(in string value) =>
        value.All(c => char.IsDigit(c));

    private static bool DigitsWithSingleSeparator(in string value, char separator)
    {
        int separators = 0;
        foreach (var c in value)
        {
            if (!char.IsDigit(c) && (c != separator)) return false;
            if (c == separator) separators++;
        }
        return (separators <= 1);
    }
}
