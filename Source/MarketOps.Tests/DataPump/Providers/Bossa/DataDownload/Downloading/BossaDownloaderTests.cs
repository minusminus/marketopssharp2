using MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using MarketOps.Tests.Mocks.HttpClientHelpers;
using RichardSzalay.MockHttp;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace MarketOps.Tests.DataPump.Providers.Bossa.DataDownload.Downloading;

[TestFixture]
internal class BossaDownloaderTests
{
    private IHttpClientFactory _factory = null!;
    private IBossaPathsConfigurationReader _configReader = null!;
    private BossaDownloader _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = Substitute.For<IHttpClientFactory>();
        _configReader = Substitute.For<IBossaPathsConfigurationReader>();
        _testObj = new BossaDownloader(_factory, _configReader);
    }

    [Test, Explicit("Long running download directly from bossa")]
    public void Get_Daily_DirectlyFromBossa__Downloads()
    {
        var configReader = new PathsConfigurationReader();
        var httpClient = new HttpClient();
        _factory.CreateClient().Returns(httpClient);
        var testObj = new BossaDownloader(_factory, configReader);
        using var resultStream = new MemoryStream();

        testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Daily, Types.StockType.Stock, "KGHM", stream => stream.CopyTo(resultStream));

        resultStream.Position = 0;
        resultStream.Length.ShouldBeGreaterThan(0);
        Console.WriteLine($"stream length: {resultStream.Length}");
        using var zip = new ZipArchive(resultStream, ZipArchiveMode.Read);
        const int checkedEntries = 10;
        Console.WriteLine($"entries count: {zip.Entries.Count}");
        Console.WriteLine($"first {checkedEntries} entries:");
        foreach (var entry in zip.Entries.Take(checkedEntries))
            Console.WriteLine($"{entry.Name} ({entry.FullName}), size: {entry.Length}, compressed size: {entry.CompressedLength}");
    }

    [Test]
    public void Get_Daily__Downloads()
    {
        const string content = "some test content";
        var contentArray = Encoding.UTF8.GetBytes(content);
        using var contentStream = new MemoryStream(contentArray);
        var httpClient = MockHttpClientManager.CreateHttpClient(CreateMessageHandler(contentStream));
        _factory.CreateClient().Returns(httpClient);
        _configReader.Read().Returns(CreateBossaPaths());
        var testObj = new BossaDownloader(_factory, _configReader);
        using var resultStream = new MemoryStream();

        testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Daily, Types.StockType.Stock, "KGHM", stream => stream.CopyTo(resultStream));

        resultStream.Position = 0;
        resultStream.Length.ShouldBeGreaterThan(0);
        var resultString = Encoding.UTF8.GetString(resultStream.ToArray());
        resultString.ShouldBe(content);

        static MockHttpMessageHandler CreateMessageHandler(MemoryStream contentStream)
        {
            var msgHandler = MockHttpClientManager.CreateMockHttpMessageHandler();
            msgHandler
                .When("*")
                .Respond(HttpStatusCode.OK, new StreamContent(contentStream));
            return msgHandler;
        }

        static BossaPaths CreateBossaPaths()
        {
            var bossaConfig = new BossaPaths
            {
                Daily = new List<DailyPathDescription>() { new DailyPathDescription() { StockType = Types.StockType.Stock, FileName = "test.test", Path = "http://test/" } }
            };
            return bossaConfig;
        }
    }

    [Test]
    public void Get_Daily_NotConfiguredStockType__Throws()
    {
        var config = new BossaPaths();
        _configReader.Read().Returns(config);

        Should.Throw<BossaDownloadException>(() => _testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Daily, Types.StockType.Stock, "", _ => { }));
    }

    [Test]
    public void Get_Ticks__Throws()
    {
        Should.Throw<ArgumentException>(() => _testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Tick, Types.StockType.Stock, "", _ => { }));
    }
}
