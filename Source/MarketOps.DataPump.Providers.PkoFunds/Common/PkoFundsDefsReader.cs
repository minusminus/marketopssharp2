using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using System.Text.Json;

namespace MarketOps.DataPump.Providers.PkoFunds.Common;

/// <summary>
/// Reads PKo funds configuration from json.
/// </summary>
internal class PkoFundsDefsReader
{
    internal const string DefsFileName = "pkofunds.defs.json";

    public PkoFundsDefs Read()
    {
        var configFilePath = GetConfigFilePath();
        return DeserializeConfigFile(configFilePath)
        ?? throw new PkoFundsConfigurationReaderException(configFilePath);
    }

    private static PkoFundsDefs? DeserializeConfigFile(string configFilePath)
    {
        try
        {
            return JsonSerializer.Deserialize<PkoFundsDefs>(File.ReadAllText(configFilePath));
        }
        catch (Exception e)
        {
            throw new PkoFundsConfigurationReaderException(configFilePath, e);
        }
    }

    private static string GetConfigFilePath() =>
        Path.Combine(Consts.ExecutingLocation, DefsFileName);
}
