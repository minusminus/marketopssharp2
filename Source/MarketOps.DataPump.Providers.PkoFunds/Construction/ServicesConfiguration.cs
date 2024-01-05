using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Buffering;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Downloading;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;
using MarketOps.DataPump.Providers.PkoFunds.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Providers.Bossa.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPkoFundsProvider(this IServiceCollection services)
    {
        var pkoFundsDefs = PkoFundsDefsReader.Read();
        services.AddSingleton(pkoFundsDefs);

        services.AddTransient<IPkoDownloader, PkoDownloader>();
        services.AddTransient<IPkoDataReader, PkoDataReader>();
        services.AddSingleton<IPkoFundsDataBuffer, PkoFundsDataBuffer>();

        services.AddTransient<IDataPumpPumpingDataProvider, PkoDataProvider>();
        return services;
    }
}
