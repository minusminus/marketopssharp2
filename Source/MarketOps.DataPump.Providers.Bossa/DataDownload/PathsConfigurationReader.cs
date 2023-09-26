using System.Reflection;
using System.Text.Json;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload;

/// <summary>
/// Reads json download paths configuuration.
/// </summary>
internal static class PathsConfigurationReader
{
    private const string PathsFileName = "bossa.paths.json";

    public static BossaPaths Read()
    {
        var configFileName = Path.Combine(Assembly.GetExecutingAssembly().Location, PathsFileName);
        var result = JsonSerializer.Deserialize<BossaPaths>(File.ReadAllText(configFileName));
        if (result is null)
            throw new JsonException($"{PathsFileName} not found or incorrect format.");
        return result;
    }
}
