using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.Tests.Mocks.Types;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.Tests.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

[TestFixture]
internal class DataDownloaderTests
{
    private const string StockName = "ADATEX";
    private const string TestContentHeader =
        "<TICKER>,<DTYYYYMMDD>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>";
    private const string TestContent = 
$@"{TestContentHeader}
{StockName},20191024,0.8000,0.8700,0.8000,0.8000,5073
{StockName},20191025,0.8000,0.8000,0.8000,0.8000,2250
{StockName},20191028,0.8000,0.8100,0.8000,0.8100,1600
{StockName},20191029,0.8100,0.8100,0.8000,0.8000,1735
{StockName},20191030,0.7800,0.7800,0.7800,0.7800,500
{StockName},20191031,0.7800,0.7800,0.7800,0.7800,1415";
    private readonly StockDefinitionShort _stockDefinitionShort = new(1, StockType.Stock, StockName, DateTime.Now);

    private IDownloadBuffer _downloadBuffer = null!;
    private ILogger<DataDownloader> _logger = null!;
    private DataDownloader _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _downloadBuffer = Substitute.For<IDownloadBuffer>();
        _logger = Substitute.For<ILogger<DataDownloader>>();
        _testObj = new DataDownloader(_downloadBuffer, _logger);
    }

    [Test]
    public void GetLines_CorrectFile__ReturnsAllLinesWithoutHeader()
    {
        _downloadBuffer.GetFile(default, default!).ReturnsForAnyArgs(new MockBufferEntry(TestContent));

        var result = _testObj
            .GetLines(MarketOps.DataPump.Common.PumpingDataRange.Daily, _stockDefinitionShort)
            .ToList();

        result.Count.ShouldBe(6);
        result.All(line => line.StartsWith(StockName)).ShouldBeTrue();

        _logger.DidNotReceiveWithAnyArgs().LogWarning(default);
    }

    [Test]
    public void GetLines_OnlyHeader__ReturnsEmptyList()
    {
        _downloadBuffer.GetFile(default, default!).ReturnsForAnyArgs(new MockBufferEntry(TestContentHeader));

        _testObj
            .GetLines(MarketOps.DataPump.Common.PumpingDataRange.Daily, _stockDefinitionShort)
            .ShouldBeEmpty();

        _logger.DidNotReceiveWithAnyArgs().LogWarning(default);
    }

    [Test]
    public void GetLines_EmptyInput__ReturnsEmptyList()
    {
        _downloadBuffer.GetFile(default, default!).ReturnsForAnyArgs(new MockBufferEntry(string.Empty));

        _testObj
            .GetLines(MarketOps.DataPump.Common.PumpingDataRange.Daily, _stockDefinitionShort)
            .ShouldBeEmpty();

        _logger.DidNotReceiveWithAnyArgs().LogWarning(default);
    }

    [Test]
    public void GetLines_NullStreamInput__ReturnsEmptyList()
    {
        _downloadBuffer.GetFile(default, default!).ReturnsForAnyArgs(new MockNullBufferEntry());
        _testObj
            .GetLines(MarketOps.DataPump.Common.PumpingDataRange.Daily, _stockDefinitionShort)
            .ShouldBeEmpty();

        _logger.ReceivedWithAnyArgs().LogWarning(default);
    }
}
