using MarketOps.Scanner.Abstractions;
using Microsoft.Extensions.Hosting;

namespace MarketOps.Scanner.Services;

internal class ScannerExecutorService : BackgroundService
{
    private readonly IScanningExecutor _scanningExecutor;
    private readonly IHostApplicationLifetime _hostLifetime;

    public ScannerExecutorService(IScanningExecutor scanningExecutor, IHostApplicationLifetime hostLifetime)
    {
        _scanningExecutor = scanningExecutor;
        _hostLifetime = hostLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await Task.Run(async () =>
        {
            await _scanningExecutor.Execute(stoppingToken);
            _hostLifetime.StopApplication();
        }, stoppingToken);
}
