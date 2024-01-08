using MarketOps.DataPump.Providers.PkoFunds.Common;

namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Gets single fund data from data array.
/// </summary>
internal static class DataFromArrayGetter
{
    public static IEnumerable<StageData> GetSingleFundDataFromLastTs(this string[][] data, int fundIndex, int lastTsIndex)
    {
        int currIndex = data.GetStartingIndex(lastTsIndex);

        while (currIndex >= 0)
        {
            yield return new(data[currIndex][PkoCsvData.DateIndex], data[currIndex][fundIndex]);
            currIndex--;
        }
    }

    private static int GetStartingIndex(this string[][] data, int lastTsIndex) => 
        (lastTsIndex >= 0)
            ? lastTsIndex - 1
            : data.Length - 1;
}
