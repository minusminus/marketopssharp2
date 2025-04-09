using Dapper;
using MarketOps.Viewer.Backend.DataReading.Dto;
using MarketOps.Viewer.Backend.Entities;
using Npgsql;

namespace MarketOps.Viewer.Backend.DataReading.Handlers;

internal class GetStocksHandler
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<GetStocksHandler> _logger;

    public GetStocksHandler(NpgsqlConnection connection, ILogger<GetStocksHandler> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<IResult> HandleGetStocksAsync()
    {
        const string sql = @"
            SELECT
                id AS Id,
                stock_short AS Symbol,
                stock_name AS StockName,
                stock_type AS TypeInt
            FROM public.at_spolki2
            WHERE enabled = true AND stock_type != -1 AND stock_short IS NOT NULL
            ORDER BY stock_short";

        try
        {
            await using (_connection)
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    await _connection.OpenAsync();

                var result = await _connection.QueryAsync<dynamic>(sql);

                return TypedResults.Ok(MapToStockInfoDto(result));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stocks");
            return Results.Problem("An error occurred while fetching stock list.", statusCode: 500);
        }
    }

    private static List<StockInfoDto> MapToStockInfoDto(IEnumerable<dynamic> result) =>
        result
        .Select(r => new StockInfoDto(
            r.id,
            r.symbol,
            r.stockname,
            ((StockType)r.typeint).ToString()
        ))
        .ToList();
}
