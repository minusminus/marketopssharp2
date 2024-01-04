using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using System.Reflection;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Config;

[TestFixture]
internal class PkoFundsDefsReaderTests
{
    private PkoFundsDefsReader _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _testObj = new PkoFundsDefsReader();
    }

    [Test]
    public void Read__ReadsCorrectly()
    {
        var result = _testObj.Read();

        result.DownloadPath.ShouldNotBeNullOrEmpty();
        result.StocksMapping.Count.ShouldBeGreaterThan(0);
    }

    [Test]
    public void Read_ConfigFileDoesNotExist__Throws()
    {
        var configFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, PkoFundsDefsReader.DefsFileName);
        var tempFilePath = configFilePath + ".x";
        File.Move(configFilePath, tempFilePath);
        try
        {
            Should.Throw<PkoFundsConfigurationReaderException>(() => _testObj.Read());
        }
        finally
        {
            File.Move(tempFilePath, configFilePath);
        }
    }
}
