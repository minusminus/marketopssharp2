using MarketOps.DataPump.Providers.PkoFunds.Common;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Verifies data.
/// For incorrect line logs error and filters it out.
/// </summary>
internal static class StageDataVerifier
{
    public static IEnumerable<StageData> Verify(this IEnumerable<StageData> input, string fundName, ILogger logger)
    {
        foreach (var data in input)
            if (IsDataCorrect(data))
                yield return data;
            else
                LogDataError(data, fundName, logger);
    }

    private static bool IsDataCorrect(StageData data)
    {
        if (string.IsNullOrWhiteSpace(data.Ts) || !OnlyDigitsAndSeparators(data.Ts, PkoCsvData.DateSeparator)) return false;
        if (!OnlyDigitsAndSeparators(data.Price, PkoCsvData.PriceSeparator)) return false;
        return true;
    }

    private static void LogDataError(StageData data, in string fundName, ILogger logger) => 
        logger.LogError("Incorrect data line for {Fund}: ts={Ts}, price={Price}", fundName, data.Ts, data.Price);

    private static bool OnlyDigitsAndSeparators(in string value, char separator) => 
        value.All(c => char.IsDigit(c) || (c == separator));
}
