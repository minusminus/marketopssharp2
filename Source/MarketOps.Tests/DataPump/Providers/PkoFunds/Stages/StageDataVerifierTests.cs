using MarketOps.DataPump.Providers.PkoFunds.Stages;
using Microsoft.Extensions.Logging;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.Stages;

[TestFixture]
internal class StageDataVerifierTests
{
    private readonly string _fundName = nameof(_fundName);
    private ILogger _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
    }

    [Test]
    public void Verify_CorrectData__ReturnsAllObjects_NothingLogged()
    {
        var input = new List<StageData>()
        {
            new StageData("2023-11-20", "1,0"),
            new StageData("2023-11-21", "2,2"),
            new StageData("2023-11-22", "3,3"),
        };

        var result = StageDataVerifier.Verify(input, _fundName, _logger).ToList();

        result.ShouldBe(input);
        _logger.DidNotReceiveWithAnyArgs().LogError(default);
    }

    [Test]
    public void Verify_IncorrectTs__IncorrectObjectsFilteredOut_ErrorsLogged()
    {
        var input = new List<StageData>()
        {
            new StageData("2023.11.20", "1"),
            new StageData("", "2"),
            new StageData("abcd", "3"),
        };

        var result = StageDataVerifier.Verify(input, _fundName, _logger).ToList();

        result.ShouldBeEmpty();
        _logger.ReceivedWithAnyArgs(input.Count).LogError(default);
    }

    [Test]
    public void Verify_IncorrectPrice__IncorrectObjectsFilteredOut_ErrorsLogged()
    {
        var input = new List<StageData>()
        {
            new StageData("2023-11-20", "1.0"),
            new StageData("2023-11-21", "2,2z"),
        };

        var result = StageDataVerifier.Verify(input, _fundName, _logger).ToList();

        result.ShouldBeEmpty();
        _logger.ReceivedWithAnyArgs(input.Count).LogError(default);
    }
}
