using Dapper;
using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.DbSchema;
using MarketOps.Common.Pg.Operations;
using MarketOps.DataPump.Common;
using MarketOps.Types;
using System.Data;

namespace MarketOps.DataPump.Storers.Pg.Operations;

/// <summary>
/// Stores PumpingData in Pg database.
/// </summary>
internal class PgDataPumpStorer : PgOperationBase, IDataPumpDataStorer
{
    public PgDataPumpStorer(IPgConnectionFactory pgConnectionFactory) : base(pgConnectionFactory)
    { }

    public void Store(IEnumerable<PumpingData> data)
    {
        using var connection = CreateAndOpenConnection();
        foreach (var pumpingData in data)
            StorePumpingData(connection, pumpingData);
    }

    private void StorePumpingData(IDbConnection connection, PumpingData pumpingData) => 
        connection.Execute(PrepareQuery(pumpingData.StockDefinition.Type), PrepareParameters(pumpingData));

    private static string PrepareQuery(StockType stockType) => $@"
insert into {TablesSelector.GetDailyTable(stockType)}
({Daily.FkStockId},
{Daily.Ts},
{Daily.Open},
{Daily.High},
{Daily.Low},
{Daily.Close},
{Daily.Volume})
values
(@{Daily.FkStockId},
@{Daily.Ts},
@{Daily.Open},
@{Daily.High},
@{Daily.Low},
@{Daily.Close},
@{Daily.Volume})";

    private DynamicParameters PrepareParameters(PumpingData pumpingData) =>
        new DynamicParameters()
            .AddInParamInt("@" + Daily.FkStockId, pumpingData.StockDefinition.Id)
            .AddInParamDateTime("@" + Daily.Ts, PrepareTs(pumpingData.Ts))
            .AddInParamNumeric("@" + Daily.Open, PrepareDecimal(pumpingData.O))
            .AddInParamNumeric("@" + Daily.High, PrepareDecimal(pumpingData.H))
            .AddInParamNumeric("@" + Daily.Low, PrepareDecimal(pumpingData.L))
            .AddInParamNumeric("@" + Daily.Close, PrepareDecimal(pumpingData.C))
            .AddInParamInt("@" + Daily.Volume, int.Parse(pumpingData.V));

    private static DateTime PrepareTs(string ts)
    {
        return new DateTime(TsPartToInt(0, 4), TsPartToInt(4, 2), TsPartToInt(6, 2));

        int TsPartToInt(int startIndex, int length) =>
            int.Parse(ts.AsSpan(startIndex, length));
    }

    private static decimal PrepareDecimal(string value) => 
        decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
}
