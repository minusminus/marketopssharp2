using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Processing;

/// <summary>
/// Buffer fo PkoFundsData.
/// </summary>
internal interface IPkoFundsDataBuffer
{
    public PkoFundsData Get();
}
