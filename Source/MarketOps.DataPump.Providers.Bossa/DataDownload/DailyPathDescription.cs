using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload;

/// <summary>
/// Bossa daily path description.
/// </summary>
internal class DailyPathDescription
{
    public StockType StockType { get; set; }
    public string Path { get; set; }
    public string FileName { get; set; }
}
