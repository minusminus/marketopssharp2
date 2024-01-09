using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.Processing;
using MarketOps.DataPump.Providers.Bossa.StockSelection;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Providers.Bossa.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterBossaProvider(this IServiceCollection services)
    {
        services.AddSingleton<IBossaPathsConfigurationReader, PathsConfigurationReader>();
        services.AddSingleton<IBossaDownloader, BossaDownloader>();
        services.AddSingleton<IDownloadBuffer, DiskBuffer>();
        services.AddTransient<IDataDownloader, DataDownloader>();

        services.AddTransient<IDataPumpPumpingDataProvider, BossaDataProvider>();
        services.AddTransient<IDataPumpStocksSelector, AllStocksSelector>();
        return services;
    }
}
