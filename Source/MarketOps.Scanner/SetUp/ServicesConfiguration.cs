using MarketOps.Common.Pg.Construction;
using MarketOps.Scanner.CliCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.Scanner.SetUp;

internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration,
        ExecutionOptions executionOptions) =>
        services
            .AddSingleton(executionOptions)
            .RegisterPostgress(configuration);
            //.RegisterPgStocksProvider()
            //.RegisterSpecifiedDataProvider(executionOptions.PumpingDataProvider)
            //.RegisterExecutor()
            //.AddHostedService<DataPumpExecutorService>();
}
