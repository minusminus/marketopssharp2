using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using System.IO.Compression;

namespace MarketOps.Tests.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

[TestFixture]
internal class DiskBufferEntryTests
{
    private const string TestContent = "some test content";
    private const string StockName = nameof(StockName);
    private const string StockFileName = $"{StockName}.mst";
    private const string NotExistingStockName = nameof(NotExistingStockName);
    private const string NotExistingStockFileName = $"{NotExistingStockName}.mst";
    private string _zipFilePath;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _zipFilePath = Path.Combine(Consts.ExecutingLocation, nameof(DiskBufferEntryTests));
        Directory.CreateDirectory(_zipFilePath);
        _zipFilePath = Path.Combine(_zipFilePath, "Test.zip");
        CreateZipFile(_zipFilePath);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Directory.Delete(Path.GetDirectoryName(_zipFilePath)!, true);
    }

    [Test]
    public void Create__CreatesBufferEntry()
    {
        using var result = DiskBufferEntry.Create(_zipFilePath, StockFileName);

        CheckBufferEntry(result);
    }

    [Test]
    public void Create_Twice__CreatesTwoReadableBufferEntries()
    {
        using var result1 = DiskBufferEntry.Create(_zipFilePath, StockFileName);
        using var result2 = DiskBufferEntry.Create(_zipFilePath, StockFileName);

        CheckBufferEntry(result1);
        CheckBufferEntry(result2);
    }

    [Test]
    public void GetStream_NotExistingStock__ReturnsNull()
    {
        using var result = DiskBufferEntry.Create(_zipFilePath, NotExistingStockFileName);

        result.ShouldNotBeNull();
        result.GetStream().ShouldBeNull();
    }

    private static void CreateZipFile(string zipPath)
    {
        using var fs = new FileStream(zipPath, FileMode.Create);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create, true);
        var entry = zip.CreateEntry(StockFileName);
        using var entryStream = entry.Open();
        using var streamWriter = new StreamWriter(entryStream);
        streamWriter.WriteLine(TestContent);
    }

    private static void CheckBufferEntry(BufferEntry result)
    {
        result.ShouldNotBeNull();
        result.GetStream()!.ReadToEnd().Trim().ShouldBe(TestContent);
    }
}
