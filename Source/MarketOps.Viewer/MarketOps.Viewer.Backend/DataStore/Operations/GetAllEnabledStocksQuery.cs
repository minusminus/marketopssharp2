using Dapper;
using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Dto;
using Npgsql;

namespace MarketOps.Viewer.Backend.DataStore.Operations;

internal class GetAllEnabledStocksQuery : IGetAllEnabledStocksQuery
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<GetAllEnabledStocksQuery> _logger;

    public GetAllEnabledStocksQuery(NpgsqlDataSource dataSource, ILogger<GetAllEnabledStocksQuery> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<IEnumerable<StockInfoQueryResultDto>> ExecuteAsync()
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
            await using var connection = await _dataSource.OpenConnectionAsync();
            return await connection.QueryAsync<StockInfoQueryResultDto>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enabled stocks list");
            throw;
        }
    }
}