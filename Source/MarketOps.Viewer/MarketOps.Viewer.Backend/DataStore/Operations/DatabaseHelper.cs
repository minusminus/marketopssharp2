using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.DataStore.Operations;

internal class DatabaseHelper : IDatabaseHelper
{
    public string? GetOhlcvTableName(StockType stockType, Timeframe timeframe)
    {
        string? prefix = timeframe switch
        {
            Timeframe.Daily => "at_dzienne",
            Timeframe.Weekly => "at_tyg",
            Timeframe.Monthly => "at_mies",
            _ => null // Nieobsługiwany timeframe
        };
        if (prefix == null) return null;

        string? suffix = stockType switch
        {
            StockType.Stock => "0",
            StockType.Index => "1",
            StockType.IndexFuture => "2",
            // Dodaj case dla typów 4, 5, 6 jeśli będą potrzebne
            _ => null // Nieobsługiwany StockType
        };
        if (suffix == null) return null;

        return $"public.{prefix}{suffix}";
    }
}