using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Storers;

/// <summary>
/// Storing mock iterating through pumping data.
/// Does not store data.
/// </summary>
internal class OnlyIterationStorer : IDataPumpPumpingDataStorer
{
    public string TempString;

    public void Store(IEnumerable<PumpingData> data)
    {
        foreach (var item in data)
            TempString = item.C;
        TempString = string.Empty;
    }
}
