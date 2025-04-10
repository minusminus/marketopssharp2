using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.DataStore.Abstractions;

internal interface IGetStockTypeQuery
{
    Task<StockType?> ExecuteAsync(int stockId);
}
