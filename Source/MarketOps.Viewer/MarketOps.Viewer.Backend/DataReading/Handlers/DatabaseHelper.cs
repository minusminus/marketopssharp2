using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.DataReading.Handlers;

internal static class DatabaseHelper
{
    public static string? GetOhlcvTableName(StockType stockType, Timeframe timeframe)
    {
        string prefix;
        switch (timeframe)
        {
            case Timeframe.Daily: prefix = "at_dzienne"; break;
            case Timeframe.Weekly: prefix = "at_tyg"; break;
            case Timeframe.Monthly: prefix = "at_mies"; break;
            default: return null; // Nieobsługiwany timeframe
        }

        string? suffix;
        switch (stockType)
        {
            case StockType.Stock: suffix = "0"; break;
            case StockType.Index: suffix = "1"; break;
            case StockType.IndexFuture: suffix = "2"; break;
            // Dodaj case dla typów 4, 5, 6 jeśli będą potrzebne
            default: return null; // Nieobsługiwany StockType
        }

        return $"public.{prefix}{suffix}";
    }
}