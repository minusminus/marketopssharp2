using Microsoft.Extensions.Configuration;

namespace MarketOps.DataPump.SetUp;

internal static class JsonFileConfiguration
{
    public static void ConfigureAppConfig(this IConfigurationBuilder configurationBuilder) =>
        configurationBuilder
            .AddJsonFile(GlobalDataPumpConsts.ConfigFileName);
}
