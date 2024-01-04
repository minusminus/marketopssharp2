using System.Text.Json.Serialization;

namespace MarketOps.DataPump.Providers.PkoFunds.Config;

/// <summary>
/// PKO funds definitions.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class PkoFundsDefs
{
    [JsonPropertyName("downloadPath")]
    public string DownloadPath { get; set; }

    [JsonPropertyName("stocksMapping")]
    public Dictionary<string, string> StocksMapping { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
