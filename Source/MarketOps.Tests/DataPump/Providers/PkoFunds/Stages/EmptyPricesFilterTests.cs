using MarketOps.DataPump.Providers.PkoFunds.Stages;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Stages;

[TestFixture]
internal class EmptyPricesFilterTests
{
    [Test]
    public void FilterOutEmptyPrices_NoEmptyPrices__ReturnsAllObjects()
    {
        var input = new List<StageData>()
        {
            new StageData("2023-11-20", "1"),
            new StageData("2023-11-21", "2"),
            new StageData("2023-11-22", "3"),
        };

        var result = EmptyPricesFilter.FilterOutEmptyPrices(input).ToList();

        result.ShouldBe(input);
    }

    [Test]
    public void FilterOutEmptyPrices_HasEmptyPrices__ReturnsNonEmptyObjects()
    {
        var input = new List<StageData>()
        {
            new StageData("2023-11-20", "1"),
            new StageData("2023-11-21", ""),
            new StageData("2023-11-22", "3"),
            new StageData("2023-11-23", " "),
            new StageData("2023-11-24", "5"),
        };

        var result = EmptyPricesFilter.FilterOutEmptyPrices(input).ToList();

        result.Count.ShouldBe(3);
        result[0].Price.ShouldBe("1");
        result[1].Price.ShouldBe("3");
        result[2].Price.ShouldBe("5");
    }

    [Test]
    public void FilterOutEmptyPrices_EmptyList__ReturnsEmptyList()
    {
        EmptyPricesFilter.FilterOutEmptyPrices(new List<StageData>()).ShouldBeEmpty();
    }
}
