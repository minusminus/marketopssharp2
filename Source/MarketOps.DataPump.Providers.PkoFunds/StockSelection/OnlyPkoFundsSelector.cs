using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.StockSelection;

/// <summary>
/// Selects only active PKO funds.
/// </summary>
internal class OnlyPkoFundsSelector : IDataPumpStocksSelector
{
    private readonly IDataPumpStocksDataProvider _stocksDataProvider;

    public OnlyPkoFundsSelector(IDataPumpStocksDataProvider stocksDataProvider)
    {
        _stocksDataProvider = stocksDataProvider;
    }

    public IEnumerable<StockDefinitionShort> SelectStocks(StockType stockType) =>
        stockType switch
        {
            StockType.InvestmentFund => _stocksDataProvider.GetAllActive(stockType, "PKO"),
            _ => Enumerable.Empty<StockDefinitionShort>(),
        };
}
