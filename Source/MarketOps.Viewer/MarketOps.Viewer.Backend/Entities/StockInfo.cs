namespace MarketOps.Viewer.Backend.Entities;

internal class StockInfo
{
    public int Id { get; set; }
    public bool Enabled { get; set; }
    public StockType Type { get; set; }
    public string? StockFullName { get; set; }
    public string StockName { get; set; } = string.Empty;
    public string? StockShort { get; set; }
    public string? StockDescription { get; set; }
    public DateTime? StartTs { get; set; }
}
