using MarketOps.Types;
using System.Text.Json.Serialization;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Types;

/// <summary>
/// Bossa daily path description.
/// </summary>
internal class DailyPathDescription
{
    [JsonPropertyName("stockType")]
    public StockType StockType { get; set; }

    [JsonPropertyName("path ")]
    public string Path { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
}
