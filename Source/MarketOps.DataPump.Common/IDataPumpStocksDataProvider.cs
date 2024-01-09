using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface providing stocks' data to pump data for.
/// </summary>
public interface IDataPumpStocksDataProvider
{
    public IEnumerable<StockDefinitionShort> GetAllActive(StockType stockType);
    public IEnumerable<StockDefinitionShort> GetAllActive(StockType stockType, string stockNamePrefix);
}
