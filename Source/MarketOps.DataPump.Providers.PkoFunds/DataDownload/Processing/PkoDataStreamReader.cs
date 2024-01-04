using MarketOps.DataPump.Providers.PkoFunds.Common;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Exceptions;

namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;

/// <summary>
/// Converts data from web to PkoFundsData.
/// </summary>
internal static class PkoDataStreamReader
{
    public static PkoFundsData Read(Stream stream)
    {
        using var reader = new StreamReader(stream);
        if (reader.EndOfStream)
            throw new PkoFundsDataException("Empty downloaded data stream");

        var namesToIndex = GetNamesToIndex(reader);
        var (data, dateToIndex) = GetData(reader);

        return new PkoFundsData(namesToIndex, dateToIndex, data);
    }

    private static Dictionary<string, int> GetNamesToIndex(StreamReader reader) =>
        reader
            .ReadLine()!
            .Split(PkoCsvData.FieldsSeparator)
            .Select((name, i) => (name, i))
            .ToDictionary(x => x.name, x => x.i);

    private static (string[][] data, Dictionary<string, int> dateToIndex) GetData(StreamReader reader)
    {
        var data = ReadAllLines(reader)
            .Select(line => line.Split(PkoCsvData.FieldsSeparator))
            .ToArray();

        var dateToIndex = data
            .Select((row, i) => (row[PkoCsvData.DataIndex], i))
            .ToDictionary(x => x.Item1, x => x.i);

        return (data, dateToIndex);
    }

    private static IEnumerable<string> ReadAllLines(StreamReader reader)
    {
        while (!reader.EndOfStream)
            yield return reader.ReadLine()!;
    }
}
