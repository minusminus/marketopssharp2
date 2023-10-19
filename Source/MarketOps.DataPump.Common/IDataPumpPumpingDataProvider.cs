using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface to generate pumps' data from external source.
/// </summary>
public interface IDataPumpPumpingDataProvider
{
    public IEnumerable<PumpingData> Get(PumpingDataRange dataRange, StockDefinitionShort stockDefinition);
}
