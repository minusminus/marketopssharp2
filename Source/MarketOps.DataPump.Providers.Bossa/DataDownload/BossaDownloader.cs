using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload;

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

    }

    private string CreateDailyFileUri(StockType stockType, string stockName)
    {

    }

    private void DownloadAndProcessStream(string uri, Action<Stream> streamProcessor)
    {
        using var client = _factory.CreateClient();
        using var stream = client.GetStreamAsync(uri).Result;
        streamProcessor(stream);
    }
}
