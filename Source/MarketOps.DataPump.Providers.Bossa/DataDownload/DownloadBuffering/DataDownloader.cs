using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Processing;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Downloads bossa files using specified download buffer.
/// </summary>
internal class DataDownloader : IDataDownloader
{
    private readonly IDownloadBuffer _downloadBuffer;
    private readonly ILogger<DataDownloader> _logger;

    public DataDownloader(IDownloadBuffer downloadBuffer, ILogger<DataDownloader> logger)
    {
        _downloadBuffer = downloadBuffer;
        _logger = logger;
    }

    public IEnumerable<string> GetLines(PumpingDataRange dataRange, StockDefinitionShort stockDefinition) =>
        dataRange switch
        {
            PumpingDataRange.Daily => GetDailyLines(stockDefinition),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };

    private IEnumerable<string> GetDailyLines(StockDefinitionShort stockDefinition)
    {
        using var bufferEntry = GetEntryFromBuffer(stockDefinition);
        using var streamReader = bufferEntry.GetStream();
        if (streamReader is null)
        {
            _logger.LogWarning("Missing data file for id={Id} [{Name}]", stockDefinition.Id, stockDefinition.Name);
            yield break;
        }
        if (!streamReader.EndOfStream)
            streamReader.ReadLine();
        while (!streamReader.EndOfStream)
            yield return streamReader.ReadLine() ?? string.Empty;
    }

    private BufferEntry GetEntryFromBuffer(StockDefinitionShort stockDefinition) => 
        _downloadBuffer.GetFile(PumpingDataRange.Daily, stockDefinition);
}
