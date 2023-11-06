using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Execution.Construction;
using MarketOps.DataPump.Providers.Bossa.Construction;
using MarketOps.DataPump.Providers.Pg.Construction;
using MarketOps.DataPump.Storers;
using MarketOps.DataPump.Storers.Pg.Construction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.SetUp;

internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration,
        bool simulateStore) =>
        services
            .AddHttpClient()
            .RegisterPostgress(configuration)
            .RegisterPgStocksProvider()
            .RegisterStorer(simulateStore)
            .RegisterBossaProvider()
            .RegisterExecutor();

    private static IServiceCollection RegisterStorer(this IServiceCollection services, bool simulateStore)
    {
        if (simulateStore)
            services.RegisterMockStorer();
        else
            services.RegisterPgStorer();

        return services;
    }

    private static IServiceCollection RegisterMockStorer(this IServiceCollection services) =>
        services.AddTransient<IDataPumpPumpingDataStorer, OnlyIterationStorer>();
}
