using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Operations;

namespace MarketOps.Viewer.Backend.DataStore.Construction;

internal static class DataStoreRegistrar
{
    public static void SetUpDataStore(this IServiceCollection services, IConfiguration configuration)
    {
        services.SetUpPostgres(configuration);

        services.AddSingleton<IDatabaseHelper, DatabaseHelper>();

        services.AddScoped<IGetStockTypeQuery, GetStockTypeQuery>();
        services.AddScoped<IGetOhlcvDataQuery, GetOhlcvDataQuery>();
        services.AddScoped<IGetAllEnabledStocksQuery, GetAllEnabledStocksQuery>();
    }

    private static void SetUpPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringParam = "PostgresConnection";

        var connectionString = configuration.GetConnectionString(ConnectionStringParam);
        services.AddNpgsqlDataSource(connectionString);
    }

}
