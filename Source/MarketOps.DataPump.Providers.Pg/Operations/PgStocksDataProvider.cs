using Dapper;
using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.DbSchema;
using MarketOps.Common.Pg.Operations;
using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Pg.Operations;

/// <summary>
/// Gets stocks data from Pg.
/// </summary>
internal class PgStocksDataProvider : PgOperationBase, IDataPumpStocksDataProvider
{
    public PgStocksDataProvider(IPgConnectionFactory pgConnectionFactory) : base(pgConnectionFactory)
    { }

    public IEnumerable<StockDefinitionShort> GetAllActive(StockType stockType)
    {
        using var connection = CreateAndOpenConnection();
        foreach (var definition in connection.Query<StockDefinitionShort>(PrepareQuery(stockType)))
            yield return definition;
    }

    private static string PrepareQuery(StockType stockType) => $@"
select s.{Stocks.Id} as ""Id"", s.{Stocks.StockType} as ""Type"", s.{Stocks.StockName} as ""Name"", max(d.{Daily.Ts}) as ""LastTs""
from {Tables.Stocks} s
left join {TablesSelector.GetDailyTable(stockType)} d on d.{Daily.FkStockId} = s.{Stocks.Id}
where s.{Stocks.Enabled} = true and s.{Stocks.StockType} = {(int)stockType}
group by s.{Stocks.Id}, s.{Stocks.StockType}, s.{Stocks.StockName}
order by s.{Stocks.Id}
";
}
