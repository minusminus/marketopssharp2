using MarketOps.Common.Pg.ConnectionFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOps.Common.Pg.Construction;

/// <summary>
/// Registers module.
/// </summary>
public static class ServicesConfiguration
{
    public static IServiceCollection RegisterPostgress(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new PostgresOptions();
        configuration.GetSection(nameof(PostgresOptions)).Bind(options);
        services.AddSingleton(options);
        services.AddTransient<IPgConnectionFactory, PgConnectionFactory>();
        //services.AddTransient<IDbConnection>(sp =>
        //{
        //    var options = sp.GetRequiredService<PostgresOptions>();
        //    return new NpgsqlConnection(options.ConnectionString);
        //});

        return services;
    }
}
