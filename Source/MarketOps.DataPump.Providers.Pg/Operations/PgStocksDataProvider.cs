using Dapper;
using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.DbSchema;
using MarketOps.Common.Pg.Operations;
using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Pg.Operations;

/// <summary>
/// Gets stocks data frmo Pg.
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
select {Stocks.Id} as ""Id"", {Stocks.StockType} as ""Type"", {Stocks.StockName} as ""Name""
from {Tables.Stocks}
where {Stocks.Enabled} = true and {Stocks.StockType} = {(int)stockType}
order by {Stocks.Id}
";
}
