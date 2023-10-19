using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Pumping executor interface.
/// </summary>
public interface IDataPumpExecutor
{
    public void Execute(params StockType[] stockTypes);
}
