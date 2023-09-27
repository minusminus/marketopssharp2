using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Processing;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Downloads bossa files using specified download buffer.
/// </summary>
internal class DataDownloader : IDataDownloader
{
    private readonly IDownloadBuffer _downloadBuffer;

    public DataDownloader(IDownloadBuffer downloadBuffer)
    {
        _downloadBuffer = downloadBuffer;
    }

    public IEnumerable<string> GetLines(PumpingDataRange dataRange, StockDefinitionShort stockDefinition) =>
        dataRange switch
        {
            PumpingDataRange.Daily => GetDailyLines(stockDefinition),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };

    private IEnumerable<string> GetDailyLines(StockDefinitionShort stockDefinition)
    {
        using var bufferEntry = _downloadBuffer.GetFile(PumpingDataRange.Daily, stockDefinition);
        using var streamReader = bufferEntry.GetStream();
        if (!streamReader.EndOfStream)
            streamReader.ReadLine();
        while (!streamReader.EndOfStream)
            yield return streamReader.ReadLine() ?? string.Empty;
    }
}
