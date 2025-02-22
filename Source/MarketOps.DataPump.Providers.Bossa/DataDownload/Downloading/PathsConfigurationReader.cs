using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Abstractions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using System.Text.Json;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Reads json download paths configuuration.
/// </summary>
internal class PathsConfigurationReader : IBossaPathsConfigurationReader
{
    internal const string PathsFileName = "bossa.paths.json";

    public BossaPaths Read()
    {
        var configFilePath = GetConfigFilePath();
        return DeserializeConfigFile(configFilePath)
        ?? throw new BossaConfigurationReaderException(configFilePath);
    }

    private static BossaPaths? DeserializeConfigFile(string configFilePath)
    {
        try
        {
            return JsonSerializer.Deserialize<BossaPaths>(File.ReadAllText(configFilePath));
        }
        catch (Exception e)
        {
            throw new BossaConfigurationReaderException(configFilePath, e);
        }
    }

    private static string GetConfigFilePath() =>
        Path.Combine(Consts.ExecutingLocation, PathsFileName);
}
