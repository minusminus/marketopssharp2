using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Storers.Pg.Operations;
using MarketOps.Tests.Mocks.PgHelpers;

namespace MarketOps.Tests.DataPump.Storers.Pg.Operations;

[TestFixture, Explicit]
internal class PgDataPumpStorerTestsDb
{
    private PostgresOptions _options;
    private PgConnectionFactory _pgConnectionFactory;
    private PgDataPumpStorer _testObj;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _options = PostgresOptionsFactory.Create();
        _pgConnectionFactory = new PgConnectionFactory(_options);
    }

    [SetUp]
    public void SetUp()
    {
        _testObj = new PgDataPumpStorer(_pgConnectionFactory);
    }

    [Test(Description = "Check db after execution")]
    public void Store__StoresInPg([Values(1, 2)] int count)
    {
        var data = Enumerable.Range(1, count)
            .Select(i =>
            {
                var stockDefinition = new Types.StockDefinitionShort(i, Types.StockType.Stock, "stock", DateTime.Now);
                return new PumpingData(PumpingDataRange.Daily, stockDefinition, "1.2345", "3.9768", "0.8700", "2.5000", "1234", "20231017");
            });

        _testObj.Store(data);
    }
}
