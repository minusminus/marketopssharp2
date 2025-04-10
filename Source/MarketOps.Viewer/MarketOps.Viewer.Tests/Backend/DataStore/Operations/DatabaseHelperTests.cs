using MarketOps.Viewer.Backend.DataStore.Operations;
using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Tests.Backend.DataStore.Operations;

[TestFixture]
internal class DatabaseHelperTests
{
    private DatabaseHelper _testObj;

    [SetUp]
    public void SetUp()
    {
        _testObj = new DatabaseHelper();
    }

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
        // Arrange (Dane wej�ciowe dostarczone przez TestCase)

        // Act
        string? result = _testObj.GetOhlcvTableName(stockType, timeframe);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedTableName);
    }

    [TestCase(StockType.Undefined, Timeframe.Daily)] // Nieobs�ugiwany StockType
    [TestCase(StockType.Undefined, Timeframe.Weekly)]
    [TestCase(StockType.Undefined, Timeframe.Monthly)]

    [TestCase(StockType.Stock, (Timeframe)99)]      // Nieistniej�cy Timeframe (przyk�ad)
    [TestCase(StockType.Index, (Timeframe)99)]
    [TestCase(StockType.IndexFuture, (Timeframe)99)]
    [TestCase(StockType.Undefined, (Timeframe)99)]
    public void GetOhlcvTableName_InvalidOrUnsupportedInputs__ReturnsNull(StockType stockType, Timeframe timeframe)
    {
        // Arrange (Dane wej�ciowe dostarczone przez TestCase)

        // Act
        string? result = _testObj.GetOhlcvTableName(stockType, timeframe);

        // Assert
        result.ShouldBeNull();
    }
}
