using Dapper;
using MarketOps.Viewer.Backend.DataReading.Dto;
using MarketOps.Viewer.Backend.Entities;
using Npgsql;
using System.Text;

namespace MarketOps.Viewer.Backend.DataReading.Handlers;

internal class GetStockDataHandler
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<GetStockDataHandler> _logger;
    public GetStockDataHandler(NpgsqlConnection connection, ILogger<GetStockDataHandler> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<IResult> HandleGetStockDataAsync(
        int stockId,
        string timeframe = "daily",
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        Timeframe requestedTimeframe = GetTimeFrame(timeframe);
        if (requestedTimeframe == Timeframe.Undefined)
            return TypedResults.BadRequest("Invalid timeframe. Use 'daily', 'weekly', or 'monthly'.");

        try
        {
            await using (_connection)
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    await _connection.OpenAsync();

                var stockType = await GetStockType(stockId);
                if (stockType == StockType.Undefined)
                {
                    _logger.LogWarning("Stock with ID {StockId} not found.", stockId);
                    return TypedResults.NotFound($"Stock with ID {stockId} not found.");
                }

                var tableName = DatabaseHelper.GetOhlcvTableName(stockType, requestedTimeframe);
                if (string.IsNullOrEmpty(tableName))
                {
                    _logger.LogWarning("Data not supported for timeframe {Timeframe} and stock type {StockType}", timeframe, stockType);
                    return TypedResults.BadRequest($"Data for timeframe '{timeframe}' and stock type '{stockType}' is not supported.");
                }

                var (query, parameters) = BuildSqlQuery(stockId, startDate, endDate, tableName);

                var data = await _connection.QueryAsync<OhlcvDataPointDto>(query, parameters);

                return TypedResults.Ok(data);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock data for ID {StockId}", stockId);
            return Results.Problem($"An error occurred while fetching data for stock ID {stockId}.", statusCode: 500);
        }
    }

    private static Timeframe GetTimeFrame(string timeframe) =>
        timeframe.ToLowerInvariant() switch
        {
            "daily" => Timeframe.Daily,
            "weekly" => Timeframe.Weekly,
            "monthly" => Timeframe.Monthly,
            _ => Timeframe.Undefined
        };

    private async Task<StockType> GetStockType(int stockId)
    {
        const string typeSql = "SELECT stock_type FROM public.at_spolki2 WHERE id = @StockId";
        var stockTypeInt = await _connection.QuerySingleOrDefaultAsync<int?>(typeSql, new { StockId = stockId });
        return stockTypeInt != null 
            ? (StockType)stockTypeInt.Value 
            : StockType.Undefined;
    }

    private static (string query, DynamicParameters parameters) BuildSqlQuery(int stockId, DateTime? startDate, DateTime? endDate, string tableName)
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
            parameters.Add("StartDate", start.Value);
        }
        if (end.HasValue)
        {
            sqlBuilder.Append("AND ts < @EndDateNextDay ");
            parameters.Add("EndDateNextDay", end.Value.AddDays(1));
        }

        sqlBuilder.Append("ORDER BY ts ASC");

        return (sqlBuilder.ToString(), parameters);
    }
}
