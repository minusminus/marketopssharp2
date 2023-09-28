using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Downloads zipped data from bossa.
/// </summary>
internal class BossaDownloader : IBossaDownloader
{
    private readonly IHttpClientFactory _factory;
    private readonly IBossaPathsConfigurationReader _configReader;
    private BossaPaths? _bossaPaths;

    public BossaDownloader(IHttpClientFactory factory, IBossaPathsConfigurationReader configReader)
    {
        _factory = factory;
        _configReader = configReader;
    }

    public void Get(PumpingDataRange dataRange, StockType stockType, string stockName, Action<Stream> streamProcessor)
    {
        string downloadUri = dataRange switch
        {
            PumpingDataRange.Daily => GetDailyFileUri(stockType),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };
        DownloadAndProcessStream(downloadUri, streamProcessor);
    }

    private string GetDailyFileUri(StockType stockType)
    {
        var definition = GetSingletonConfig().Daily?.Find(x => x.StockType == stockType);
        if (definition is null)
            throw new BossaDownloadException(stockType);
        return BuildDailyUri(definition);
    }

    private void DownloadAndProcessStream(string uri, Action<Stream> streamProcessor)
    {
        using var client = _factory.CreateClient();
        using var stream = client.GetStreamAsync(uri).Result;
        streamProcessor(stream);
    }
    private static string BuildDailyUri(DailyPathDescription definition) => 
        new Uri(new Uri(definition.Path), definition.FileName).ToString();

    private BossaPaths GetSingletonConfig()
    {
        _bossaPaths ??= _configReader.Read();
        return _bossaPaths;
    }
}
