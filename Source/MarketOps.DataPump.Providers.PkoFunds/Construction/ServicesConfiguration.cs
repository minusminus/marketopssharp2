using MarketOps.DataPump.Common;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Providers.Bossa.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPkoFundsProvider(this IServiceCollection services)
    {
        //services.AddSingleton<IBossaPathsConfigurationReader, PathsConfigurationReader>();
        //services.AddSingleton<IBossaDownloader, BossaDownloader>();
        //services.AddSingleton<IDownloadBuffer, DiskBuffer>();
        //services.AddTransient<IDataDownloader, DataDownloader>();
        //services.AddTransient<IDataPumpPumpingDataProvider, BossaDataProvider>();
        return services;
    }
}
