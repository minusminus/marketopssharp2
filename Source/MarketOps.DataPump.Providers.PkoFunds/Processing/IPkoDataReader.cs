using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.Processing;

/// <summary>
/// Interface to read PkoFundsData from web.
/// </summary>
internal interface IPkoDataReader
{
    public PkoFundsData Read();
}
