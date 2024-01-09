using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;

/// <summary>
/// Interface to download csv data from pko.
/// </summary>
internal interface IPkoDownloader
{
    public PkoFundsData Get(string uri, Func<Stream, PkoFundsData> streamProcessor);
}
