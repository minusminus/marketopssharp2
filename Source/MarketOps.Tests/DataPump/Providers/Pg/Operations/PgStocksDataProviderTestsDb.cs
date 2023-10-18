﻿using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.Common.Pg.Construction;
using MarketOps.DataPump.Providers.Pg.Operations;
using MarketOps.Tests.Mocks.PgHelpers;

namespace MarketOps.Tests.DataPump.Providers.Pg.Operations;

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

    [Test(Description = "Check db after execution")]
    public void GetAllActive__GetFromPg()
    {
        var result = _testObj.GetAllActive(Types.StockType.Stock).ToList();

        Console.WriteLine($"Results count: {result.Count}");
        foreach (var item in result)
            Console.WriteLine($"{item.Id}: {item.Name} [{item.Type}]");
    }
}
