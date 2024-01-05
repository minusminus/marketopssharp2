using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Buffering;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;
using MarketOps.DataPump.Providers.PkoFunds.Processing;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.DataDownload.Buffering;

[TestFixture]
internal class PkoFundsDataBufferTests
{
    private readonly PkoFundsData _data = new(new Dictionary<string, int>(), new Dictionary<string, int>(), new string[0][]);
    private IPkoDataReader _pkoDataReader = null!;
    private PkoFundsDataBuffer _testObj = null!;

    [SetUp]
    public void SetUp()
    {
        _pkoDataReader = Substitute.For<IPkoDataReader>();
        _pkoDataReader.Read().ReturnsForAnyArgs(_data);
        _testObj = new(_pkoDataReader);
    }

    [Test]
    public void Get__ReturnsSingletonObject([Values(1, 2, 5)] int callCount)
    {
        var result = Enumerable.Range(1, callCount)
            .Select(_ => _testObj.Get())
            .ToList();

        _pkoDataReader.Received(1).Read();
        result.All(x => x.Equals(_data)).ShouldBeTrue();
    }
}
