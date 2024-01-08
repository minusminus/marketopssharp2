using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Finds indexes of last ts for specified stock in funds data.
/// 
/// If ts is not found returns -1, assuming processing is required from the beginning fo data.
/// </summary>
internal static class DataIndexFinder
{
    public static (int fundIndex, int tsIndex) FindIndexes(this PkoFundsData data, PkoFundsDefs pkoFundsDefs, 
        StockDefinitionShort stockDefinition)
    {
        var fundIndex = data.FindFundIndex(pkoFundsDefs, stockDefinition);
        var tsIndex = data.FindTsIndex(stockDefinition);

        return (fundIndex, tsIndex);
    }

    private static int FindFundIndex(this PkoFundsData data, PkoFundsDefs pkoFundsDefs, StockDefinitionShort stockDefinition) => 
        pkoFundsDefs.StocksMapping.TryGetValue(stockDefinition.Name, out var fundName)
            ? data.FundNameToIndex.FindIndexInDict(fundName)
            : PkoCsvData.NotFoundDataIndex;

    private static int FindTsIndex(this PkoFundsData data, StockDefinitionShort stockDefinition)
    {
        string lastTsValue = stockDefinition.LastTs.ToString(PkoCsvData.DateFormat);
        return data.DateToIndex.FindIndexInDict(lastTsValue);
    }

    private static int FindIndexInDict(this IReadOnlyDictionary<string, int> dict, string key) => 
        dict.TryGetValue(key, out var result)
            ? result
            : PkoCsvData.NotFoundDataIndex;
}
