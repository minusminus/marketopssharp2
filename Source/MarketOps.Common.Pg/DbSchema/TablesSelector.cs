using MarketOps.Types;

namespace MarketOps.Common.Pg.DbSchema;

public static class TablesSelector
{
    public static string GetDailyTable(StockType stockType) => 
        $"{Tables.Daily}{(int)stockType}";
}
