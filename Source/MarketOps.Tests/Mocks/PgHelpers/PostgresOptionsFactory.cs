using MarketOps.Common.Pg.Construction;
using Microsoft.Extensions.Configuration;

namespace MarketOps.Tests.Mocks.PgHelpers;

internal static class PostgresOptionsFactory
{
    public static PostgresOptions Create()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(GlobalTestConsts.ConfigFileName)
            .Build();

        return configurationBuilder.GetRequiredSection(nameof(PostgresOptions)).Get<PostgresOptions>()
            ?? throw new Exception($"{nameof(PostgresOptions)} not found in {GlobalTestConsts.ConfigFileName}");
    }
}
