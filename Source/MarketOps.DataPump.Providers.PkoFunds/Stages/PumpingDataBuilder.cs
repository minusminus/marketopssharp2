using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Creates PumpingData from funds data.
/// </summary>
internal static class PumpingDataBuilder
{
    public static IEnumerable<PumpingData> ToPumpingData(this IEnumerable<StageData> input, PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        foreach (var data in input)
            yield return BuildPumpingData(data, dataRange, stockDefinition);
    }

    private static PumpingData BuildPumpingData(StageData data, PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        var priceValue = PriceWithDot(data.Price);
        return new(
            dataRange,
            stockDefinition,
            priceValue,
            priceValue,
            priceValue,
            priceValue,
            "0",
            NoSeparatorInTs(data.Ts));
    }

    private static string PriceWithDot(in string priceValue) =>
        priceValue.Replace(PkoCsvData.PriceSeparator, '.');

    private static string NoSeparatorInTs(in string tsValue) => 
        string.Concat(tsValue.Where(c => c != PkoCsvData.DateSeparator));
}
