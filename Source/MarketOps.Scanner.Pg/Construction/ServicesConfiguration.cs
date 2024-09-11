using MarketOps.Scanner.Common;
using MarketOps.Scanner.Pg.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.Scanner.Pg.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPgStocksProvider(this IServiceCollection services)
    {
        services.AddTransient<IScannerStockDataProvider, PgStocksDataProvider>();
        return services;
    }
}
