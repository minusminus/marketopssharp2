using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;
using System.Reflection;
using System.Text.Json;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Reads json download paths configuuration.
/// </summary>
internal static class PathsConfigurationReader
{
    internal const string PathsFileName = "bossa.paths.json";

    public static BossaPaths Read()
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
        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, PathsFileName);
}
