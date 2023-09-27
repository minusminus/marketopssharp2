using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;

internal class BossaDownloadException : DataPumpException
{
    public BossaDownloadException(StockType stockType)
        : base($"Download definition not found for stock type: {stockType}")
    { }
}
