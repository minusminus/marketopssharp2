using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.Construction;
using MarketOps.Scanner.Pg.Operations;
using MarketOps.Tests.Mocks.PgHelpers;
using MarketOps.Types;

namespace MarketOps.Tests.Scanner.Pg.Operations;

[TestFixture, Explicit]
internal class PgStocksDataProviderTestsDb
{
    private PostgresOptions _options;
    private PgConnectionFactory _pgConnectionFactory;
    private PgStocksDataProvider _testObj;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _options = PostgresOptionsFactory.Create();
        _pgConnectionFactory = new PgConnectionFactory(_options);
    }

    [SetUp]
    public void SetUp()
    {
        _testObj = new PgStocksDataProvider(_pgConnectionFactory);
    }

    [Test]
    public async Task GetStockDefinition_ExistingStock__GetsFromPg()
    {
        var result = await _testObj.GetStockDefinitionAsync("Stock1");

        result.ShouldNotBeNull();
        Console.WriteLine(result);
    }

    [Test]
    public async Task GetStockDefinition_NotExistingStock__ReturnsNull()
    {
        var result = await _testObj.GetStockDefinitionAsync("Some not existing stock");

        result.ShouldBeNull();
    }

    [Test]
    public async Task GetStockData__ReturnsDataInCorrectOrder()
    {
        var stock = new StockDefinitionShort(1, StockType.Stock, string.Empty, DateTime.Now);

        var result = await _testObj.GetStockDataAsync(stock);

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        for (int i = 1; i < result.Count; i++)
            result.Ts[i].ShouldBeLessThan(result.Ts[i - 1]);
    }
}
