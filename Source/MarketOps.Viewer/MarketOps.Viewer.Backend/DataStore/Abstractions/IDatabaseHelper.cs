using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.DataStore.Abstractions;

internal interface IDatabaseHelper
{
    string? GetOhlcvTableName(StockType stockType, Timeframe timeframe);
}
