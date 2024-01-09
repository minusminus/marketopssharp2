namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Filters out objects with empty price values.
/// </summary>
internal static class EmptyPricesFilter
{
    public static IEnumerable<StageData> FilterOutEmptyPrices(this IEnumerable<StageData> input) => 
        input.Where(x => !string.IsNullOrWhiteSpace(x.Price));
}
