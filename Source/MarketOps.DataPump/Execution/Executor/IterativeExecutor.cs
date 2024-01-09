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
    private readonly IDataPumpStocksSelector _stocksSelector;
    private readonly IDataPumpPumpingDataProvider _pumpingDataProvider;
    private readonly IDataPumpPumpingDataStorer _pumpingDataStorer;
    private readonly ILogger<IterativeExecutor> _logger;

    public IterativeExecutor(IDataPumpStocksSelector stocksSelector, IDataPumpPumpingDataProvider pumpingDataProvider,
        IDataPumpPumpingDataStorer pumpingDataStorer, ILogger<IterativeExecutor> logger)
    {
        _stocksSelector = stocksSelector;
        _pumpingDataProvider = pumpingDataProvider;
        _pumpingDataStorer = pumpingDataStorer;
        _logger = logger;
    }

    public void Execute(CancellationToken stoppingToken, params StockType[] stockTypes)
    {
        _logger.LogInformation("[{ExecutorName}] started processing", nameof(IterativeExecutor));
        foreach (var stockType in stockTypes)
        {
            if (stoppingToken.IsCancellationRequested) break;
            _logger.LogInformation("[{ExecutorName}] processing stock type: {StockType}", nameof(IterativeExecutor), stockType);
            PumpData(stockType, stoppingToken);
        }
        _logger.LogInformation("[{ExecutorName}] finished processing", nameof(IterativeExecutor));
    }

    private void PumpData(StockType stockType, CancellationToken stoppingToken)
    {
        var stocksData = _stocksSelector.SelectStocks(stockType);
        foreach (var stockData in stocksData)
        {
            if (stoppingToken.IsCancellationRequested) break;
            _logger.LogInformation("[{ExecutorName}] processing data file for id={Id} [{Name}] from last ts {Ts}",
                nameof(IterativeExecutor), stockData.Id, stockData.Name, stockData.LastTs.ToString("yyyy-MM-dd"));
            var pumpingData = _pumpingDataProvider.Get(PumpingDataRange.Daily, stockData);
            StoreData(pumpingData, stoppingToken);
        }
    }

    private void StoreData(IEnumerable<PumpingData> pumpingData, CancellationToken stoppingToken)
    {
        try
        {
            _pumpingDataStorer.Store(pumpingData, stoppingToken);
        } catch(Exception e)
        {
            _logger.LogError(e, "[{ExecutorName}] exception on storing data: {ExceptionMessage}", nameof(IterativeExecutor), e.Message);
        }
    }
}
