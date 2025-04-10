using MarketOps.Viewer.Backend.DataStore.Construction;
using MarketOps.Viewer.Backend.Operations;

namespace MarketOps.Viewer.Backend.SetUp;

internal static class ServicesSetup
{
    public static IServiceCollection SetUpServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.SetUpDataStore(configuration);

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
}
