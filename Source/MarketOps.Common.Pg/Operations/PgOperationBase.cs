using MarketOps.Common.Pg.ConnectionFactory;
using System.Data;

namespace MarketOps.Common.Pg.Operations;

/// <summary>
/// Base class for Pg operations.
/// </summary>
public abstract class PgOperationBase
{
    private readonly IPgConnectionFactory _pgConnectionFactory;

    protected PgOperationBase(IPgConnectionFactory pgConnectionFactory)
    {
        _pgConnectionFactory = pgConnectionFactory;
    }

    protected IDbConnection CreateAndOpenConnection()
    {
        var connection = CreateConnection();
        connection.Open();
        return connection;
    }

    private IDbConnection CreateConnection() =>
        _pgConnectionFactory.Create();
}
