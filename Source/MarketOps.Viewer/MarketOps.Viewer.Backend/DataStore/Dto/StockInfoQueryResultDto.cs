namespace MarketOps.Viewer.Backend.DataStore.Dto;

internal class StockInfoQueryResultDto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty; // Zmieniono z StockShort na Symbol
    public string StockName { get; set; } = string.Empty;
    public int TypeInt { get; set; }
}
