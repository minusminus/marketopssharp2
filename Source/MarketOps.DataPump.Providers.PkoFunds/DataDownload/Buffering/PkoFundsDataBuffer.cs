using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Processing;

namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Buffering;

/// <summary>
/// PkoFundsData buffer as a singleton.
/// </summary>
internal class PkoFundsDataBuffer : IPkoFundsDataBuffer
{
    private readonly IPkoDataReader _pkoDataReader;
    private PkoFundsData? _data;

    public PkoFundsDataBuffer(IPkoDataReader pkoDataReader)
    {
        _pkoDataReader = pkoDataReader;
    }

    public PkoFundsData Get()
    {
        _data ??= _pkoDataReader.Read();
        return _data;
    }
}
