using MarketOps.Types;
using System.Text.Json.Serialization;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Types;

/// <summary>
/// Bossa daily path description.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class DailyPathDescription
{
    [JsonPropertyName("stockType")]
    public StockType StockType { get; set; }

    [JsonPropertyName("path ")]
    public string Path { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
