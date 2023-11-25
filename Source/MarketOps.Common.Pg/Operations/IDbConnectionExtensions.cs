using System.Data;

namespace MarketOps.Common.Pg.Operations;

/// <summary>
/// IDbConnection extensions.
/// </summary>
public static class IDbConnectionExtensions
{
    public static IDbCommand CreateCommand(this IDbConnection dbConnection, string query)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandType = CommandType.Text;
        dbCommand.CommandText = query;
        return dbCommand;
    }
}