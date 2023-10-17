using MarketOps.Common.Pg.Construction;
using Npgsql;
using System.Data;

namespace MarketOps.Common.Pg.ConnectionFactory;

/// <summary>
/// Pg connections factory.
/// </summary>
internal class PgConnectionFactory : IPgConnectionFactory
{
    private readonly PostgresOptions _options;

    public PgConnectionFactory(PostgresOptions options)
    {
        _options = options;
    }

    public IDbConnection Create() => 
        new NpgsqlConnection(_options.ConnectionString);
}
