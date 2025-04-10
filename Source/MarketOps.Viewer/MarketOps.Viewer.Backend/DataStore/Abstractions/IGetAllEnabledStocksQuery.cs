using MarketOps.Viewer.Backend.DataStore.Dto;

namespace MarketOps.Viewer.Backend.DataStore.Abstractions;

internal interface IGetAllEnabledStocksQuery
{
    // Użyjemy nadal wewnętrznego DTO dla wyniku zapytania
    Task<IEnumerable<StockInfoQueryResultDto>> ExecuteAsync();
}