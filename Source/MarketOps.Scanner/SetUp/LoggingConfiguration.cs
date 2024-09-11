using MarketOps.Scanner.Common;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MarketOps.Scanner.SetUp;

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
