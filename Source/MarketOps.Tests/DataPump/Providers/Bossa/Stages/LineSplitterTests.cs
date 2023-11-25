using MarketOps.DataPump.Providers.Bossa.Stages;

namespace MarketOps.Tests.DataPump.Providers.Bossa.Stages;

[TestFixture]
internal class LineSplitterTests
{
    [TestCase("", new string[] { "" })]
    [TestCase("abcd", new string[] {"abcd"})]
    [TestCase("abcd,efgh", new string[] { "abcd", "efgh" })]
    [TestCase("abcd,efgh,123.4", new string[] { "abcd", "efgh", "123.4" })]
    public void Split__SplitsCorrectly(string value, string[] expected)
    {
        var input = new List<string>() { value };

        var result = LineSplitter.Split(input).ToList();

        result.Count.ShouldBe(1);
        result[0].ShouldBe(expected);
    }
}
