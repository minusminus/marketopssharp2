using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.Common;
using Microsoft.Extensions.Configuration;

namespace MarketOps.Tests.Mocks.PgHelpers;

internal static class PostgresOptionsFactory
{
    public static PostgresOptions Create()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile(Consts.ConfigFileName)
            .Build();

        return configurationBuilder.GetRequiredSection(nameof(PostgresOptions)).Get<PostgresOptions>()
            ?? throw new Exception($"{nameof(PostgresOptions)} not found in {Consts.ConfigFileName}");
    }
}
