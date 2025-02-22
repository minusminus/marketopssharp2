using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;

/// <summary>
/// Interface for downloading files from bossa.
/// </summary>
internal interface IBossaDownloader
{
    public void Get(PumpingDataRange dataRange, StockType stockType, string stockName, Action<Stream> streamProcessor);
}
