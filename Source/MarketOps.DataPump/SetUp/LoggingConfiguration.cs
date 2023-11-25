using MarketOps.DataPump.Common;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MarketOps.DataPump.SetUp;

internal static class LoggingConfiguration
{
    public static void ConfigureLogging(this ILoggingBuilder loggingBuilder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(Consts.ExecutingLocation, "DataPumpLogs.log"),
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning);

        loggingBuilder
            .ClearProviders()
            .AddSerilog(loggerConfiguration.CreateLogger());
    }
}
