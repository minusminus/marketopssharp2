using Serilog;

namespace MarketOps.Viewer.Backend.SetUp;

internal static class InfrastructureSetUp
{
    public static void UseInfrastructure(this IApplicationBuilder appBuilder, IHostEnvironment environment)
    {
        appBuilder.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (environment.IsDevelopment())
        {
            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI();
        }

        appBuilder.UseCors("AllowAnyOrigin");

        appBuilder.UseAuthorization();
    }
}
