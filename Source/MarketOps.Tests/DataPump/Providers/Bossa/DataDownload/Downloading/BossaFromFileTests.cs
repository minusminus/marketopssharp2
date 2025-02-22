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
internal class BossaFromFileTests
{
    private IBossaPathsConfigurationReader _configReader = null!;
    private BossaFromFile _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _configReader = Substitute.For<IBossaPathsConfigurationReader>();
        _testObj = new BossaFromFile(_configReader);
    }

    [Test]
    public void Get_Daily__GetsFile()
    {
        const string dataFileName = "test.test";
        const string content = "some test content";
        var contentArray = Encoding.UTF8.GetBytes(content);
        File.WriteAllBytes(Path.Combine(_testObj.PredownloadedPath, dataFileName), contentArray);
        _configReader.Read().Returns(CreateBossaPaths());
        var testObj = new BossaFromFile(_configReader);
        using var resultStream = new MemoryStream();

        testObj.Get(MarketOps.DataPump.Common.PumpingDataRange.Daily, Types.StockType.Stock, "KGHM", stream => stream.CopyTo(resultStream));

        resultStream.Position = 0;
        resultStream.Length.ShouldBeGreaterThan(0);
        var resultString = Encoding.UTF8.GetString(resultStream.ToArray());
        resultString.ShouldBe(content);

        static BossaPaths CreateBossaPaths()
        {
            var bossaConfig = new BossaPaths
            {
                Daily = new List<DailyPathDescription>() { new DailyPathDescription() { StockType = Types.StockType.Stock, FileName = dataFileName, Path = "http://test/" } }
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
