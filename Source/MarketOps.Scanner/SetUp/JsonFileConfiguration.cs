using MarketOps.Scanner.Common;
using Microsoft.Extensions.Configuration;

namespace MarketOps.Scanner.SetUp;

internal static class JsonFileConfiguration
{
    public static void ConfigureAppConfig(this IConfigurationBuilder configurationBuilder) =>
        configurationBuilder
            .AddJsonFile(Consts.ConfigFileName);
}
