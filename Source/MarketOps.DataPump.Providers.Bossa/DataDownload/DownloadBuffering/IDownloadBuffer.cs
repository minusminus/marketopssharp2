using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Bossa download buffering interface.
/// </summary>
internal interface IDownloadBuffer
{
    public StreamReader GetFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition);
}
