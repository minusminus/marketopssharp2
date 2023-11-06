using MarketOps.Common.Pg.ConnectionFactory;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.Storers.Pg.Operations;
using System.Data;

namespace MarketOps.Tests.DataPump.Storers.Pg.Operations;

[TestFixture]
internal class PgDataPumpStorerTests
{
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private IDbCommand _dbCommand = null!;
    private IDbConnection _dbConnection = null!;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private IPgConnectionFactory _pgConnectionFactory = null!;
    private PgDataPumpStorer _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _dbCommand = Substitute.For<IDbCommand>();
        _dbConnection = Substitute.For<IDbConnection>();
        _dbConnection.CreateCommand().Returns(_dbCommand);
        _pgConnectionFactory = Substitute.For<IPgConnectionFactory>();
        _pgConnectionFactory.Create().Returns(_dbConnection);
        _testObj = new PgDataPumpStorer(_pgConnectionFactory);
    }

    [Test]
    public void Store__ExecutesCorrectly([Values(1, 2)] int count)
    {
        var data = GenerateTestData(count);

        _testObj.Store(data, CancellationToken.None);

        _dbConnection.ReceivedWithAnyArgs(count).CreateCommand();
        _dbCommand.ReceivedWithAnyArgs(count).ExecuteNonQuery();
    }

    [Test]
    public void Store_TokenCancelledBeforeCall__DoesNotCallDb()
    {
        var data = GenerateTestData(2);
        using CancellationTokenSource tokenSource = new();
        var token = tokenSource.Token;
        tokenSource.Cancel();

        _testObj.Store(data, token);

        _dbConnection.DidNotReceiveWithAnyArgs().CreateCommand();
        _dbCommand.DidNotReceiveWithAnyArgs().ExecuteNonQuery();
    }

    [Test]
    public void Store_EmptyList__DoesNotCallDb()
    {
        _testObj.Store(new List<PumpingData>(), CancellationToken.None);

        _dbConnection.DidNotReceiveWithAnyArgs().CreateCommand();
    }

    private static IEnumerable<PumpingData> GenerateTestData(int count)
    {
        var stockDefinition = new Types.StockDefinitionShort(1, Types.StockType.Stock, "stock", DateTime.Now);
        var pumpingData = new PumpingData(PumpingDataRange.Daily, stockDefinition, "1.2345", "3.9768", "0.8700", "2.5000", "1234", "20231017");
        return Enumerable.Range(1, count).Select(_ => pumpingData);
    }
}
