using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.PkoFunds.Config;
using System.Reflection;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Config;

[TestFixture]
internal class PkoFundsDefsReaderTests
{
    [Test]
    public void Read__ReadsCorrectly()
    {
        var result = PkoFundsDefsReader.Read();

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
            Should.Throw<PkoFundsConfigurationReaderException>(() => PkoFundsDefsReader.Read());
        }
        finally
        {
            File.Move(tempFilePath, configFilePath);
        }
    }
}
