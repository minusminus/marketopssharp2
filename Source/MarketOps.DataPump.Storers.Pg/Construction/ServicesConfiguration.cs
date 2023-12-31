﻿using MarketOps.DataPump.Common;
using MarketOps.DataPump.Storers.Pg.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.DataPump.Storers.Pg.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPgStorer(this IServiceCollection services)
    {
        services.AddTransient<IDataPumpPumpingDataStorer, PgDataPumpStorer>();
        return services;
    }
}
