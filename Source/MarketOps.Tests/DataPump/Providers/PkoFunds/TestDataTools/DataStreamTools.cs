namespace MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;

internal static class DataStreamTools
{
    public static Stream CreateDataStream(List<string> data)
    {
        var result = new MemoryStream();

        using (var writer = new StreamWriter(result, leaveOpen: true))
            foreach (var item in data)
                writer.WriteLine(item);

        result.Position = 0;
        return result;
    }
}
