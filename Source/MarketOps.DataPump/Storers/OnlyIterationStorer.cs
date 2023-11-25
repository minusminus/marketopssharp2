using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Storers;

/// <summary>
/// Storing mock iterating through pumping data.
/// Does not store data.
/// </summary>
internal class OnlyIterationStorer : IDataPumpPumpingDataStorer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string TempString;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public void Store(IEnumerable<PumpingData> data, CancellationToken stoppingToken)
    {
        foreach (var item in data)
        {
            if (stoppingToken.IsCancellationRequested) return;
            TempString = item.C;
        }
        TempString = string.Empty;
    }
}
