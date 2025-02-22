using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Provides previously downloaded zipped data from bossa.
/// Files downloaded to specified folder by hand.
/// </summary>
internal class BossaFromFile : IBossaDownloader
{
    private readonly IBossaPathsConfigurationReader _configReader;
    private BossaPaths? _bossaPaths;

    internal readonly string PredownloadedPath = Path.Combine(Consts.ExecutingLocation, "BossaFiles");

    public BossaFromFile(IBossaPathsConfigurationReader configReader)
    {
        _configReader = configReader;
        if (!Directory.Exists(PredownloadedPath))
            Directory.CreateDirectory(PredownloadedPath);
    }

    public void Get(PumpingDataRange dataRange, StockType stockType, string stockName, Action<Stream> streamProcessor)
    {
        string dataFilePath = dataRange switch
        {
            PumpingDataRange.Daily => GetDailyFilePath(stockType),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };
        ProcessDataFile(dataFilePath, streamProcessor);
    }

    private string GetDailyFilePath(StockType stockType)
    {
        var definition = GetSingletonConfig().Daily?.Find(x => x.StockType == stockType);
        if (definition is null)
            throw new BossaDownloadException(stockType);
        return BuildDailyFilePath(definition);
    }

    private string BuildDailyFilePath(DailyPathDescription definition) =>
        Path.Combine(PredownloadedPath, definition.FileName);

    private void ProcessDataFile(string filePath, Action<Stream> streamProcessor)
    {
        using var stream = File.OpenRead(filePath);
        streamProcessor(stream);
    }

    private BossaPaths GetSingletonConfig()
    {
        _bossaPaths ??= _configReader.Read();
        return _bossaPaths;
    }
}
