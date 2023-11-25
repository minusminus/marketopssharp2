using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using System.Reflection;

namespace MarketOps.Tests.DataPump.Providers.Bossa.DataDownload.Downloading;

[TestFixture]
internal class PathsConfigurationReaderTests
{
    private PathsConfigurationReader _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _testObj = new PathsConfigurationReader();
    }

    [Test]
    public void Read__ReadsCorrectly()
    {
        var result = _testObj.Read();

        result.Daily.ShouldNotBeNull();
        result.Daily.Count.ShouldBe(6);
    }

    [Test]
    public void Read_ConfigFileDoesNotExist__Throws()
    {
        var configFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, PathsConfigurationReader.PathsFileName);
        var tempFilePath = configFilePath + ".x";
        File.Move(configFilePath, tempFilePath);
        try
        {
            Should.Throw<BossaConfigurationReaderException>(() => _testObj.Read());
        }
        finally
        {
            File.Move(tempFilePath, configFilePath);
        }
    }
}
