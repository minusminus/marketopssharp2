using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Pg.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Providers.Pg.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPgStocksProvider(this IServiceCollection services)
    {
        services.AddTransient<IDataPumpStocksDataProvider, PgStocksDataProvider>();
        return services;
    }
}
