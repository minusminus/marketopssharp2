using MarketOps.Viewer.Backend.Entities;
using MarketOps.Viewer.Backend.DataReading.Handlers;

namespace MarketOps.Viewer.Tests.Backend.DataReading.Handlers;

[TestFixture]
internal class DatabaseHelperTests
{
    [TestCase(StockType.Stock, Timeframe.Daily, "public.at_dzienne0")]
    [TestCase(StockType.Index, Timeframe.Daily, "public.at_dzienne1")]
    [TestCase(StockType.IndexFuture, Timeframe.Daily, "public.at_dzienne2")]

    [TestCase(StockType.Stock, Timeframe.Weekly, "public.at_tyg0")]
    [TestCase(StockType.Index, Timeframe.Weekly, "public.at_tyg1")]
    [TestCase(StockType.IndexFuture, Timeframe.Weekly, "public.at_tyg2")]

    [TestCase(StockType.Stock, Timeframe.Monthly, "public.at_mies0")]
    [TestCase(StockType.Index, Timeframe.Monthly, "public.at_mies1")]
    [TestCase(StockType.IndexFuture, Timeframe.Monthly, "public.at_mies2")]
    public void GetOhlcvTableName_ValidInputs__ReturnsCorrectTableName(StockType stockType, Timeframe timeframe, string expectedTableName)
    {
        // Arrange (Dane wejœciowe dostarczone przez TestCase)

        // Act
        string? result = DatabaseHelper.GetOhlcvTableName(stockType, timeframe);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedTableName);
    }

    [TestCase(StockType.Undefined, Timeframe.Daily)] // Nieobs³ugiwany StockType
    [TestCase(StockType.Undefined, Timeframe.Weekly)]
    [TestCase(StockType.Undefined, Timeframe.Monthly)]

    [TestCase(StockType.Stock, (Timeframe)99)]      // Nieistniej¹cy Timeframe (przyk³ad)
    [TestCase(StockType.Index, (Timeframe)99)]
    [TestCase(StockType.IndexFuture, (Timeframe)99)]
    [TestCase(StockType.Undefined, (Timeframe)99)]
    public void GetOhlcvTableName_InvalidOrUnsupportedInputs__ReturnsNull(StockType stockType, Timeframe timeframe)
    {
        // Arrange (Dane wejœciowe dostarczone przez TestCase)

        // Act
        string? result = DatabaseHelper.GetOhlcvTableName(stockType, timeframe);

        // Assert
        result.ShouldBeNull();
    }
}
