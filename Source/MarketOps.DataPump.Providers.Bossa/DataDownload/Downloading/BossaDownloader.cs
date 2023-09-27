using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Downloads zipped data from bossa.
/// </summary>
internal class BossaDownloader
{
    private readonly IHttpClientFactory _factory;
    private readonly BossaPaths _bossaPaths = PathsConfigurationReader.Read();

    public BossaDownloader(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public void Get(PumpingDataRange dataRange, StockType stockType, string stockName, Action<Stream> streamProcessor)
    {
        string downloadUri = dataRange switch
        {
            PumpingDataRange.Daily => CreateDailyFileUri(stockType),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };
        DownloadAndProcessStream(downloadUri, streamProcessor);
    }

    private string CreateDailyFileUri(StockType stockType)
    {
        var definition = _bossaPaths.Daily.Find(x => x.StockType == stockType);
        if (definition is null)
            throw new BossaDownloadException(stockType);
        return new Uri(new Uri(definition.Path), definition.FileName).ToString();
    }

    private void DownloadAndProcessStream(string uri, Action<Stream> streamProcessor)
    {
        using var client = _factory.CreateClient();
        using var stream = client.GetStreamAsync(uri).Result;
        streamProcessor(stream);
    }
}
