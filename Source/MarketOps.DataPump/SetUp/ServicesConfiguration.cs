using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.Execution.Construction;
using MarketOps.DataPump.Providers.Bossa.Construction;
using MarketOps.DataPump.Providers.Pg.Construction;
using MarketOps.DataPump.Storers.Pg.Construction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.SetUp;

internal static class ServicesConfiguration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .RegisterPostgress(configuration)
            .RegisterPgStocksProvider()
            .RegisterPgStorer()
            .RegisterBossaProvider()
            .RegisterExecutor();
}
