using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Downloading;

/// <summary>
/// Downloads csv data from pko.
/// </summary>
internal class PkoDownloader : IPkoDownloader
{
    private readonly IHttpClientFactory _factory;

    public PkoDownloader(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public PkoFundsData Get(string uri, Func<Stream, PkoFundsData> streamProcessor)
    {
        using var client = _factory.CreateClient();
        using var stream = client.GetStreamAsync(uri).Result;
        return streamProcessor(stream);
    }
}
