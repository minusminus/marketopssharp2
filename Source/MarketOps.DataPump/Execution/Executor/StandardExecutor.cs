using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Execution.Executor;

/// <summary>
/// Data pump executor.
/// </summary>
internal class StandardExecutor : IDataPumpExecutor
{
    private readonly IDataPumpStocksDataProvider _stocksDataProvider;
    private readonly IDataPumpPumpingDataProvider _pumpingDataProvider;
    private readonly IDataPumpPumpingDataStorer _pumpingDataStorer;

    public StandardExecutor(IDataPumpStocksDataProvider stocksDataProvider, IDataPumpPumpingDataProvider pumpingDataProvider,
        IDataPumpPumpingDataStorer pumpingDataStorer)
    {
        _stocksDataProvider = stocksDataProvider;
        _pumpingDataProvider = pumpingDataProvider;
        _pumpingDataStorer = pumpingDataStorer;
    }

    public void Execute(params StockType[] stockTypes)
    {
        foreach (var stockType in stockTypes)
            PumpData(stockType);
    }

    private void PumpData(StockType stockType)
    {
        var stocksData = _stocksDataProvider.GetAllActive(stockType);
        foreach (var stockData in stocksData)
        {
            var pumpingData = _pumpingDataProvider.Get(PumpingDataRange.Daily, stockData);
            _pumpingDataStorer.Store(pumpingData);
        }
    }
}
