using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Downloading;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.DataDownload.Downloading;

[TestFixture]
internal class PkoDownloaderTests
{
    private const string DownloadPath = "https://www.pkotfi.pl/media_files/tfi/notowania_pl.csv";

    [Test, Explicit("Long running download directly from pko")]
    public void Get__GetsDataAndCallsProcessor()
    {
        using var httpClient = new HttpClient();
        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient().Returns(httpClient);
        var testObj = new PkoDownloader(factory);
        using var resultStream = new MemoryStream();

        var result = testObj.Get(DownloadPath, stream =>
        {
            stream.CopyTo(resultStream);
            return new PkoFundsData(new Dictionary<string, int>(), new Dictionary<string, int>(), new string[0][]);
        });

        result.ShouldNotBeNull();
        resultStream.Length.ShouldBeGreaterThan(0);
    }
}
