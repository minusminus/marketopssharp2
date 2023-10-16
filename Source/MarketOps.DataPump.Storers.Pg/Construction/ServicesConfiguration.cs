using MarketOps.DataPump.Common;
using MarketOps.DataPump.Storers.Pg.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Storers.Pg.Construction;

/// <summary>
/// Registers module.
/// </summary>
internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterBossaProvider(this IServiceCollection services)
    {
        services.AddTransient<IDataPumpDataStorer, PgDataPumpStorer>();
        return services;
    }
}
