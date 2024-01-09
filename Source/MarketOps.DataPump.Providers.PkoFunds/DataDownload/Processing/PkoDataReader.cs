using MarketOps.DataPump.Providers.PkoFunds.Config;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Processing;

namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;

/// <summary>
/// Reads data from web and creates PkoFundsData.
/// </summary>
internal class PkoDataReader : IPkoDataReader
{
    private readonly PkoFundsDefs _pkoFundsDefs;
    private readonly IPkoDownloader _pkoDownloader;

    public PkoDataReader(PkoFundsDefs pkoFundsDefs, IPkoDownloader pkoDownloader)
    {
        _pkoFundsDefs = pkoFundsDefs;
        _pkoDownloader = pkoDownloader;
    }

    public PkoFundsData Read() =>
        _pkoDownloader.Get(_pkoFundsDefs.DownloadPath, stream => PkoDataStreamReader.Read(stream));
}
