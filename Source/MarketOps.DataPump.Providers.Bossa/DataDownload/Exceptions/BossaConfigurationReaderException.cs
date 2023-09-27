using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;

internal class BossaConfigurationReaderException : DataPumpException
{
    private readonly string _configurationFilePath;

    public BossaConfigurationReaderException(string configurationFilePath)
        : base(GetMessage(configurationFilePath))
    {
        _configurationFilePath = configurationFilePath;
    }

    public BossaConfigurationReaderException(string configurationFilePath, Exception innerException)
        : base(GetMessage(configurationFilePath), innerException)
    {
        _configurationFilePath = configurationFilePath;
    }

    private static string GetMessage(in string configurationFilePath) =>
        $"[{configurationFilePath}] not found or in incorrect format.";
}
