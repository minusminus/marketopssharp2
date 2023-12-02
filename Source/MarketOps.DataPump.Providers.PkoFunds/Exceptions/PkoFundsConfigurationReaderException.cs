using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;

internal class PkoFundsConfigurationReaderException : DataPumpException
{
    public readonly string ConfigurationFilePath;

    public PkoFundsConfigurationReaderException(string configurationFilePath)
        : base(GetMessage(configurationFilePath))
    {
        ConfigurationFilePath = configurationFilePath;
    }

    public PkoFundsConfigurationReaderException(string configurationFilePath, Exception innerException)
        : base(GetMessage(configurationFilePath), innerException)
    {
        ConfigurationFilePath = configurationFilePath;
    }

    private static string GetMessage(in string configurationFilePath) =>
        $"[{configurationFilePath}] not found or in incorrect format.";
}
