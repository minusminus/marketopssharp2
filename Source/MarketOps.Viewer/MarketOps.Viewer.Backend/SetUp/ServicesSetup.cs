using MarketOps.Viewer.Backend.DataReading.Handlers;
using Npgsql;

namespace MarketOps.Viewer.Backend.SetUp;

internal static class ServicesSetup
{
    public static IServiceCollection SetUpServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.SetUpPostgres(configuration);

        services.AddScoped<GetStocksHandler>();
        services.AddScoped<GetStockDataHandler>();
        
        services.AddLogging();

        // Add services to the container.
        services.AddAuthorization();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin", policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        return services;
    }

    private static void SetUpPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringParam = "PostgresConnection";

        var connectionString = configuration.GetConnectionString(ConnectionStringParam);
        services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));
    }
}
