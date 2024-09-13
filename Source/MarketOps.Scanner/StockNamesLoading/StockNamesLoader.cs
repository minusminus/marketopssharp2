using MarketOps.Scanner.Abstractions;

namespace MarketOps.Scanner.StockNamesLoading;

/// <summary>
/// Loads names of stocks from file.
/// 
/// Splits lines by ';' and ','.
/// </summary>
internal class StockNamesLoader : IStockNamesLoader
{
    private static readonly char[] SplitChars = [';', ','];

    public string[] GetStockNames(string filePath)
    {
        using var reader = File.OpenText(filePath);
        return ReadAllNames(reader).ToArray();
    }

    private IEnumerable<string> ReadAllNames(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var splitedNames = line!.Split(SplitChars, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in splitedNames)
                yield return name;
        }
    }
}
