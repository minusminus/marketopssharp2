using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Exceptions;
using MarketOps.DataPump.Providers.PkoFunds.Stages;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.PkoFunds.Processing;

/// <summary>
/// Data provider for pko funds.
/// </summary>
internal class PkoDataProvider : IDataPumpPumpingDataProvider
{
    private readonly IPkoFundsDataBuffer _dataBuffer;
    private readonly PkoFundsDefs _pkoFundsDefs;
    private readonly ILogger<PkoDataProvider> _logger;

    public PkoDataProvider(IPkoFundsDataBuffer dataBuffer, PkoFundsDefs pkoFundsDefs, ILogger<PkoDataProvider> logger)
    {
        _dataBuffer = dataBuffer;
        _pkoFundsDefs = pkoFundsDefs;
        _logger = logger;
    }

    public IEnumerable<PumpingData> Get(PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        var data = _dataBuffer.Get();
        var (fundIndex, tsIndex) = FindDataStartingIndex(data, stockDefinition);

        return (fundIndex != PkoCsvData.NotFoundDataIndex)
            ? CreatePumpingData(fundIndex, tsIndex, data, dataRange, stockDefinition)
            : Enumerable.Empty<PumpingData>();
    }

    private (int fundIndex, int tsIndex) FindDataStartingIndex(PkoFundsData data, StockDefinitionShort stockDefinition)
    {
        var result = data.FindIndexes(_pkoFundsDefs, stockDefinition);

        if (result.fundIndex == PkoCsvData.NotFoundDataIndex)
            _logger.LogWarning("Missing data for id={Id} [{Name}]", stockDefinition.Id, stockDefinition.Name);

        return result;
    }

    private IEnumerable<PumpingData> CreatePumpingData(int fundIndex, int tsIndex, PkoFundsData data, PumpingDataRange dataRange, StockDefinitionShort stockDefinition) => 
        data.Data
            .GetSingleFundDataFromLastTs(fundIndex, tsIndex)
            .FilterOutEmptyPrices()
            .Verify(stockDefinition.Name, _logger)
            .ToPumpingData(dataRange, stockDefinition);
}
