using System.Text.Json.Serialization;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Types;

/// <summary>
/// Bossa paths definitions.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class BossaPaths
{
    [JsonPropertyName("daily")]
    public List<DailyPathDescription> Daily { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
