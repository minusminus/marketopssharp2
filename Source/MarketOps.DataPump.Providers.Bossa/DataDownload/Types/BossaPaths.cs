using System.Text.Json.Serialization;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Types;

/// <summary>
/// Bossa paths definitions.
/// </summary>
internal class BossaPaths
{
    [JsonPropertyName("daily")]
    public List<DailyPathDescription> Daily { get; set; }
}
