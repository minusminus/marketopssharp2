using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Buffers downloaded files on disk.
/// </summary>
internal class DiskBuffer : IDownloadBuffer
{
    private readonly BossaDownloader _downloader;

    public DiskBuffer(BossaDownloader downloader)
    {
        _downloader = downloader;
    }

    public StreamReader GetFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition) =>
        dataRange switch
        {
            PumpingDataRange.Daily => GetDailyFile(stockDefinition),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };


    private StreamReader GetDailyFile(StockDefinitionShort stockDefinition)
    {
        throw new NotImplementedException();
    }
}
