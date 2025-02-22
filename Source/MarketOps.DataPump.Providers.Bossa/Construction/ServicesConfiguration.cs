using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.Processing;
using MarketOps.DataPump.Providers.Bossa.StockSelection;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Providers.Bossa.Construction;

/// <summary>
/// Registers module.
/// 
/// FromFile copies previously downloaded file (by hand) from folder to buffer.
/// It's ineffective, but was fast and easy to implement in current infrastructure.
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

    public static IServiceCollection RegisterBossaFromFileProvider(this IServiceCollection services)
    {
        services.AddSingleton<IBossaPathsConfigurationReader, PathsConfigurationReader>();
        services.AddSingleton<IBossaDownloader, BossaFromFile>();
        services.AddSingleton<IDownloadBuffer, DiskBuffer>();
        services.AddTransient<IDataDownloader, DataDownloader>();

        services.AddTransient<IDataPumpPumpingDataProvider, BossaDataProvider>();
        services.AddTransient<IDataPumpStocksSelector, AllStocksSelector>();
        return services;
    }
}
