using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;

/// <summary>
/// Bossa download buffering interface.
/// </summary>
internal interface IDownloadBuffer
{
    public BufferEntry GetFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition);
}
