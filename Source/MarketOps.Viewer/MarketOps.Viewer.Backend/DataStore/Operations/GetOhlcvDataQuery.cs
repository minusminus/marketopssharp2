using Dapper;
using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Dto;
using Npgsql;
using System.Data;
using System.Text;

namespace MarketOps.Viewer.Backend.DataStore.Operations;

internal class GetOhlcvDataQuery : IGetOhlcvDataQuery
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<GetOhlcvDataQuery> _logger;

    public GetOhlcvDataQuery(NpgsqlDataSource dataSource, ILogger<GetOhlcvDataQuery> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<IEnumerable<OhlcvDataPointDto>> ExecuteAsync(int stockId, string tableName, DateTime? startDate, DateTime? endDate)
    {
        var (query, parameters) = BuildOhlcvSqlQuery(stockId, startDate, endDate, tableName);
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            return await connection.QueryAsync<OhlcvDataPointDto>(query, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching OHLCV data for Stock ID {StockId} from table {TableName}", stockId, tableName);
            throw;
        }
    }

    private static (string query, DynamicParameters parameters) BuildOhlcvSqlQuery(int stockId, DateTime? startDate, DateTime? endDate, string tableName)
    {
        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append($"SELECT ts AS Timestamp, open, high, low, close, volume FROM {tableName} WHERE fk_id_spolki = @StockId ");

        var parameters = new DynamicParameters();
        parameters.Add("StockId", stockId);

        var start = startDate?.Date;
        var end = endDate?.Date;

        if (start.HasValue)
        {
            sqlBuilder.Append("AND ts >= @StartDate ");
            parameters.Add("StartDate", start.Value, DbType.Date);
        }
        if (end.HasValue)
        {
            sqlBuilder.Append("AND ts <= @EndDate ");
            parameters.Add("EndDate", end.Value, DbType.Date);
        }

        sqlBuilder.Append("ORDER BY ts ASC");

        return (sqlBuilder.ToString(), parameters);
    }
}
