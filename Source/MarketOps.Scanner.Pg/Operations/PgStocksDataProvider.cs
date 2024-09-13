using Dapper;
using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.DbSchema;
using MarketOps.Common.Pg.Operations;
using MarketOps.Scanner.Common;
using MarketOps.Types;

namespace MarketOps.Scanner.Pg.Operations;

/// <summary>
/// Provides stocks data for scanning from Pg.
/// </summary>
internal class PgStocksDataProvider : PgOperationBase, IScannerStockDataProvider
{
    public PgStocksDataProvider(IPgConnectionFactory pgConnectionFactory) : base(pgConnectionFactory)
    { }

    public async Task<StockDefinitionShort?> GetStockDefinitionAsync(string stockName)
    {
        var query = $@"
select s.{Stocks.Id} as ""Id"", s.{Stocks.StockType} as ""Type"", s.{Stocks.StockName} as ""Name"", s.{Stocks.StartTs} as ""LastTs""
from {Tables.Stocks} s
where s.{Stocks.StockName} = @StockName";

        var parameters = new DynamicParameters()
            .AddInParamString("@StockName", stockName);

        using var connection = CreateAndOpenConnection();
        return await connection.QuerySingleOrDefaultAsync<StockDefinitionShort>(query, parameters);
    }

    public async Task<StockData> GetStockDataAsync(StockDefinitionShort stockDefinition)
    {
        var query = $@"
select *
from {TablesSelector.GetDailyTable(stockDefinition.Type)}
where {Daily.FkStockId} = @Id
order by {Daily.Ts} desc";

        var parameters = new DynamicParameters()
            .AddInParamInt("@Id", stockDefinition.Id);

        using var connection = CreateAndOpenConnection();
        using var reader = await connection.ExecuteReaderAsync(query, parameters);
        return PricesDataReader.ReadStockData(reader);
    }
}
