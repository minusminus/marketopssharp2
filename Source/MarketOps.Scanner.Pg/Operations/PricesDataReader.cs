using MarketOps.Common.Pg.DbSchema;
using MarketOps.Scanner.Common;
using System.Data;

namespace MarketOps.Scanner.Pg.Operations;

/// <summary>
/// Temporal prices data to read from Pg and convert to StockData.
/// </summary>
internal static class PricesDataReader
{
    private class PricesTemporalData
    {
        public readonly List<float> O = [];
        public readonly List<float> H = [];
        public readonly List<float> L = [];
        public readonly List<float> C = [];
        public readonly List<Int64> V = [];
        public readonly List<DateTime> Ts = [];
    }

    private record FieldsIndexes(int O, int H, int L, int C, int V, int Ts)
    {
        public static FieldsIndexes GetIndexes(IDataReader reader) =>
            new(reader.GetOrdinal(Daily.Open),
                reader.GetOrdinal(Daily.High),
                reader.GetOrdinal(Daily.Low),
                reader.GetOrdinal(Daily.Close),
                reader.GetOrdinal(Daily.Volume),
                reader.GetOrdinal(Daily.Ts));
    }

    public static StockData ReadStockData(IDataReader reader)
    {
        var indexes = FieldsIndexes.GetIndexes(reader);
        var temporalData = ReadData(reader, indexes);
        return temporalData.ToStockData();
    }

    private static PricesTemporalData ReadData(IDataReader reader, FieldsIndexes indexes)
    {
        var result = new PricesTemporalData();
        while (reader.Read())
        {
            result.O.Add(reader.GetFloat(indexes.O));
            result.H.Add(reader.GetFloat(indexes.H));
            result.L.Add(reader.GetFloat(indexes.L));
            result.C.Add(reader.GetFloat(indexes.C));
            result.V.Add(reader.GetInt64(indexes.V));
            result.Ts.Add(reader.GetDateTime(indexes.Ts));
        }
        return result;
    }

    private static StockData ToStockData(this PricesTemporalData temporalData) => 
        new(
            temporalData.O.Count,
            temporalData.O.ToArray(),
            temporalData.H.ToArray(),
            temporalData.L.ToArray(),
            temporalData.C.ToArray(),
            temporalData.V.ToArray(),
            temporalData.Ts.ToArray());
}
