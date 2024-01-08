using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface selecting stocks' data to process.
/// </summary>
public interface IDataPumpStocksSelector
{
    public IEnumerable<StockDefinitionShort> SelectStocks(StockType stockType);
}
