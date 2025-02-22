using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using MarketOps.Types;
using System.IO.Compression;

namespace MarketOps.Tests.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

[TestFixture]
internal class DiskBufferTests
{
    private const string StockName = nameof(StockName);
    private readonly StockDefinitionShort _stockDefinitionShort = new(1, StockType.Stock, StockName, DateTime.Now);
    private readonly MemoryStream _archiveStream = new();

    private IBossaDownloader _downloader = null!;
    private DiskBuffer _testObj = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        CreateZipFileStream();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _archiveStream.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _downloader = Substitute.For<IBossaDownloader>();
        _downloader.
            When(x => x.Get(Arg.Any<PumpingDataRange>(), Arg.Any<StockType>(), Arg.Any<string>(), Arg.Any<Action<Stream>>()))
            .Do(x => ProcessArchiveStream(x.ArgAt<Action<Stream>>(3)));
        _testObj = new DiskBuffer(_downloader);
    }

    [Test]
    public void GetFiles_Daily__ReturnsBufferEntry_AllowsMultipleZipAccess([Values(1, 2)] int callsCount)
    {
        List<BufferEntry> entries = new();
        for (int i = 0; i < callsCount; i++)
        {
            var result = _testObj.GetFile(PumpingDataRange.Daily, _stockDefinitionShort);

            result.ShouldNotBeNull();
            entries.Add(result);
        }
        entries.ForEach(x => x.Dispose());

        _downloader.ReceivedWithAnyArgs(1).Get(default, default, default!, default!);
    }

    [Test]
    public void GetFile_Ticks__Throws()
    {
        Should.Throw<ArgumentException>(() => _testObj.GetFile(PumpingDataRange.Tick, _stockDefinitionShort));
    }

    private void CreateZipFileStream()
    {
        using var zip = new ZipArchive(_archiveStream, ZipArchiveMode.Create, true);
        var entry = zip.CreateEntry($"{StockName}.mst");
        using var entryStream = entry.Open();
        using var streamWriter = new StreamWriter(entryStream);
        streamWriter.WriteLine("some test content");
    }

    private void ProcessArchiveStream(Action<Stream> streamProcessor)
    {
        _archiveStream.Position = 0;
        streamProcessor(_archiveStream);
    }
}
