using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.StockSelection;

/// <summary>
/// Select all active stocks of provided type.
/// </summary>
internal class AllStocksSelector : IDataPumpStocksSelector
{
    private readonly IDataPumpStocksDataProvider _stocksDataProvider;

    public AllStocksSelector(IDataPumpStocksDataProvider stocksDataProvider)
    {
        _stocksDataProvider = stocksDataProvider;
    }

    public IEnumerable<StockDefinitionShort> SelectStocks(StockType stockType) => 
        _stocksDataProvider.GetAllActive(stockType);
}
