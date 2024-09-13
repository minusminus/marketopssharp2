using MarketOps.Common.Pg.Construction;
using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Execution;
using MarketOps.Scanner.Pg.Construction;
using MarketOps.Scanner.ResultProcessing;
using MarketOps.Scanner.ScannersLoading;
using MarketOps.Scanner.StockNamesLoading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.Scanner.SetUp;

internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration,
        ScanningOptions scanningOptions) =>
        services
            .AddSingleton(scanningOptions)
            .RegisterPostgress(configuration)
            .RegisterPgStocksProvider()
            .AddTransient<IStockNamesLoader, StockNamesLoader>()
            .AddTransient<IScannersFactory, ScannersFactory>()
            .AddTransient<IScanResultProcessor, SingleFileResultProcessor>()
            .AddTransient<IScanningExecutor, SimpleExecutor>();

            //.AddHostedService<DataPumpExecutorService>();
}
