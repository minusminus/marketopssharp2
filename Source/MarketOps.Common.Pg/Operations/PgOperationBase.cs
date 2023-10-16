using MarketOps.Common.Pg.Construction;
using Npgsql;
using System.Data;

namespace MarketOps.Common.Pg.Operations;

/// <summary>
/// Base class for Pg operations.
/// </summary>
public abstract class PgOperationBase
{
    private readonly PostgresOptions _options;

    protected PgOperationBase(PostgresOptions options)
    {
        _options = options;
    }

    protected void VoidOperator(Action<IDbConnection> operation)
    {
        using var connection = CreateConnection();
        connection.Open();
        operation(connection);
    }

    protected T SingleOperator<T>(Func<IDbConnection, T> operation)
    {
        using var connection = CreateConnection();
        connection.Open();
        return operation(connection);
    }

    protected IEnumerable<T> EnumerableOperator<T>(Func<IDbConnection, IEnumerable<T>> operation)
    {
        using var connection = CreateConnection();
        connection.Open();
        foreach (var item in operation(connection))
            yield return item;
    }

    private IDbConnection CreateConnection() => 
        new NpgsqlConnection(_options.ConnectionString);
}
