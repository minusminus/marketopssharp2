using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Processing;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Downloads bossa files using previously downloaded data unzipped in specified folder.
/// </summary>
internal class DataDownloaderUnzipped : IDataDownloader
{
    internal readonly string PredownloadedPath = Path.Combine(Consts.ExecutingLocation, "BossaFiles");

    private readonly ILogger<DataDownloaderUnzipped> _logger;

    public DataDownloaderUnzipped(ILogger<DataDownloaderUnzipped> logger)
    {
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
        using var fileStream = GetFileDataStream(stockDefinition);
        if (fileStream is null)
        {
            _logger.LogWarning("Missing data file for id={Id} [{Name}]", stockDefinition.Id, stockDefinition.Name);
            yield break;
        }
        using var streamReader = new StreamReader(fileStream);
        if (!streamReader.EndOfStream)
            streamReader.ReadLine();
        while (!streamReader.EndOfStream)
            yield return streamReader.ReadLine() ?? string.Empty;
    }

    private FileStream? GetFileDataStream(StockDefinitionShort stockDefinition)
    {
        var filePath = Path.Combine(PredownloadedPath, $"{stockDefinition.Name}.mst");
        if (!File.Exists(filePath)) return null;
        return File.OpenRead(filePath);
    }
}
