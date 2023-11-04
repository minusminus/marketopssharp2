using MarketOps.DataPump.Common;
using MarketOps.DataPump.Execution.Executor;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Execution.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterExecutor(this IServiceCollection services)
    {
        services.AddTransient<IDataPumpExecutor, IterativeExecutor>();
        return services;
    }
}
