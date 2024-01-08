using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Exceptions;
using MarketOps.DataPump.Providers.PkoFunds.Stages;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Processing;

/// <summary>
/// Data provider for pko funds.
/// </summary>
internal class PkoDataProvider : IDataPumpPumpingDataProvider
{
    private readonly IPkoFundsDataBuffer _dataBuffer;
    private readonly PkoFundsDefs _pkoFundsDefs;

    public PkoDataProvider(IPkoFundsDataBuffer dataBuffer, PkoFundsDefs pkoFundsDefs)
    {
        _dataBuffer = dataBuffer;
        _pkoFundsDefs = pkoFundsDefs;
    }

    public IEnumerable<PumpingData> Get(PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        var data = _dataBuffer.Get();
        var (fundIndex, tsIndex) = FindDataStartingIndex(data, stockDefinition);

        return CreatePumpingData(fundIndex, tsIndex, data, dataRange, stockDefinition);
    }

    private (int fundIndex, int tsIndex) FindDataStartingIndex(PkoFundsData data, StockDefinitionShort stockDefinition)
    {
        var result = data.FindIndexes(_pkoFundsDefs, stockDefinition);

        return (result.fundIndex != PkoCsvData.NotFoundDataIndex)
            ? result
            : throw new PkoFundsFundNotFoundException(stockDefinition.Name);
    }

    private static IEnumerable<PumpingData> CreatePumpingData(int fundIndex, int tsIndex, PkoFundsData data, PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        data.Data
            .GetSingleFundDataFromLastTs(fundIndex, tsIndex)
            .FilterOutEmptyPrices();

        tsIndex--;
        while (tsIndex >= 0)
        {
            var priceValue = data.Data[tsIndex][fundIndex];
            if (!string.IsNullOrEmpty(priceValue))
                yield return new(
                    dataRange,
                    stockDefinition,
                    priceValue,
                    priceValue,
                    priceValue,
                    priceValue,
                    "0",
                    data.Data[tsIndex][PkoCsvData.DateIndex]);

            tsIndex--;
        }
    }
}
