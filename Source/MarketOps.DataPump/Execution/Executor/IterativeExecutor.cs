using MarketOps.DataPump.Common;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Execution.Executor;

/// <summary>
/// Data pump standard executor.
/// Executes all stock types and data in iterative order without prallelism.
/// </summary>
internal class IterativeExecutor : IDataPumpExecutor
{
    private readonly IDataPumpStocksDataProvider _stocksDataProvider;
    private readonly IDataPumpPumpingDataProvider _pumpingDataProvider;
    private readonly IDataPumpPumpingDataStorer _pumpingDataStorer;
    private readonly ILogger<IterativeExecutor> _logger;

    public IterativeExecutor(IDataPumpStocksDataProvider stocksDataProvider, IDataPumpPumpingDataProvider pumpingDataProvider,
        IDataPumpPumpingDataStorer pumpingDataStorer, ILogger<IterativeExecutor> logger)
    {
        _stocksDataProvider = stocksDataProvider;
        _pumpingDataProvider = pumpingDataProvider;
        _pumpingDataStorer = pumpingDataStorer;
        _logger = logger;
    }

    public void Execute(params StockType[] stockTypes)
    {
        _logger.LogInformation("[{ExecutorName}] started processing", nameof(IterativeExecutor));
        foreach (var stockType in stockTypes)
        {
            _logger.LogInformation("[{ExecutorName}] processing stock type: {StockType}", nameof(IterativeExecutor), stockType);
            PumpData(stockType);
        }
        _logger.LogInformation("[{ExecutorName}] finished processing", nameof(IterativeExecutor));
    }

    private void PumpData(StockType stockType)
    {
        var stocksData = _stocksDataProvider.GetAllActive(stockType);
        foreach (var stockData in stocksData)
        {
            _logger.LogInformation("[{ExecutorName}] processing data file for id={Id} [{Name}] from last ts {Ts}",
                nameof(IterativeExecutor), stockData.Id, stockData.Name, stockData.LastTs.ToString("yyyy-MM-dd"));
            var pumpingData = _pumpingDataProvider.Get(PumpingDataRange.Daily, stockData);
            _pumpingDataStorer.Store(pumpingData);
        }
    }
}
