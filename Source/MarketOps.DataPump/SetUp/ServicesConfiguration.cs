using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.CliCommands;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Execution.Construction;
using MarketOps.DataPump.Providers.Bossa.Construction;
using MarketOps.DataPump.Providers.Pg.Construction;
using MarketOps.DataPump.Services;
using MarketOps.DataPump.Storers;
using MarketOps.DataPump.Storers.Pg.Construction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.SetUp;

internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration,
        ExecutionOptions executionOptions) =>
        services
            .AddHttpClient()
            .AddSingleton(executionOptions)
            .RegisterPostgress(configuration)
            .RegisterPgStocksProvider()
            .RegisterProperStorer(executionOptions.SimulateStore)
            .RegisterSpecifiedDataProvider(executionOptions.PumpingDataProvider)
            .RegisterExecutor()
            .AddHostedService<DataPumpExecutorService>();

    private static IServiceCollection RegisterProperStorer(this IServiceCollection services, bool simulateStore)
    {
        if (simulateStore)
            services.RegisterMockStorer();
        else
            services.RegisterPgStorer();

        return services;
    }

    private static IServiceCollection RegisterSpecifiedDataProvider(this IServiceCollection services, PumpingDataProvider pumpingDataProvider)
    {
        switch (pumpingDataProvider)
        {
            //case PumpingDataProvider.Bossa: services.RegisterBossaProvider(); break;
            case PumpingDataProvider.Bossa: throw new Exception($"{PumpingDataProvider.Bossa} provider is turned off");
            case PumpingDataProvider.BossaFromfile: services.RegisterBossaFromFileProvider(); break;
            case PumpingDataProvider.PkoFunds: services.RegisterPkoFundsProvider(); break;
            default: throw new Exception($"Unkonwn pumping data provider: {pumpingDataProvider}");
        }
        return services;
    }

    private static IServiceCollection RegisterMockStorer(this IServiceCollection services) =>
        services.AddTransient<IDataPumpPumpingDataStorer, OnlyIterationStorer>();
}
