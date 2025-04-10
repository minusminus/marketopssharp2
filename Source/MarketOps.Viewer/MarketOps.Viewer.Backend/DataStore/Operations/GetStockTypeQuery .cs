using Dapper;
using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.Entities;
using Npgsql;

namespace MarketOps.Viewer.Backend.DataStore.Operations;

internal class GetStockTypeQuery : IGetStockTypeQuery
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<GetStockTypeQuery> _logger; // Logger specyficzny dla tej klasy

    public GetStockTypeQuery(NpgsqlDataSource dataSource, ILogger<GetStockTypeQuery> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    public async Task<StockType?> ExecuteAsync(int stockId)
    {
        const string typeSql = "SELECT stock_type FROM public.at_spolki2 WHERE id = @StockId";
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync();
            var stockTypeInt = await connection.QuerySingleOrDefaultAsync<int?>(typeSql, new { StockId = stockId });
            return stockTypeInt.HasValue ? (StockType)stockTypeInt.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock type for ID {StockId}", stockId);
            throw; // Rzucamy dalej, handler obsłuży
        }
    }
}
