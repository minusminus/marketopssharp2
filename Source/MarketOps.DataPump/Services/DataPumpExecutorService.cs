using MarketOps.DataPump.CliCommands;
using MarketOps.DataPump.Common;
using Microsoft.Extensions.Hosting;

namespace MarketOps.DataPump.Services;

internal class DataPumpExecutorService : BackgroundService
{
    private readonly IDataPumpExecutor _dataPumpExecutor;
    private readonly ExecutionOptions _executionOptions;

    public DataPumpExecutorService(IDataPumpExecutor dataPumpExecutor, ExecutionOptions executionOptions)
    {
        _dataPumpExecutor = dataPumpExecutor;
        _executionOptions = executionOptions;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => 
        Task.Run(() => _dataPumpExecutor.Execute(stoppingToken, _executionOptions.StockTypes), stoppingToken);
}
