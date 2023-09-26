namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface to store stocks' data.
/// </summary>
public interface IDataPumpDataStorer
{
    public void Store(IEnumerable<PumpingData> data);
}
