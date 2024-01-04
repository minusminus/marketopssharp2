using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Processing;

/// <summary>
/// Data provider for pko funds.
/// </summary>
internal class PkoDataProvider : IDataPumpPumpingDataProvider
{
    private PkoFundsData? _data;

    private readonly IPkoDataReader _pkoDataReader;

    public PkoDataProvider(IPkoDataReader pkoDataReader)
    {
        _pkoDataReader = pkoDataReader;
    }

    public IEnumerable<PumpingData> Get(PumpingDataRange dataRange, StockDefinitionShort stockDefinition)
    {
        throw new NotImplementedException();
    }

    private PkoFundsData GetData()
    {
        _data ??= _pkoDataReader.Read();
        return _data;
    }
}
