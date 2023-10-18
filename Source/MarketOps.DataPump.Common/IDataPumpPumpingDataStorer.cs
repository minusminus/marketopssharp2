namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface to store stocks' data.
/// </summary>
public interface IDataPumpPumpingDataStorer
{
    public void Store(IEnumerable<PumpingData> data);
}
