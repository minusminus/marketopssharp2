using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using System.IO.Compression;

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
        //false.ShouldBeTrue();
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
