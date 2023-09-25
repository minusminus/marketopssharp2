using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Stages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace MarketOps.Tests.DataPump.Providers.Bossa.Stages;

[TestFixture]
internal class LinesVerifierTests
{
    private ILogger logger = null!;

    private static readonly List<string> ohlcIncorrectValues = new()
    {
        "0.2980.",
        "",
        "   ",
        "0.298X",
        "abcdef",
        "a.cdef",
    };

    [SetUp]
    public void SetUp()
    {
        logger = Substitute.For<ILogger>();
    }

    [Test]
    public void Verify_Daily__AllCorrect__ReturnsAll()
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", "0.2980", "0.3280", "0.2980", "0.3000", "24000"},
            new string[] {"ADATEX", "20221202", "0.3000", "0.3020", "0.3000", "0.3000", "78665"},
            new string[] {"ADATEX", "20221205", "0.3240", "0.3800", "0.3000", "0.3000", "326891"},
            new string[] {"ADATEX", "20221206", "0.3000", "0.3000", "0.2920", "0.2920", "30215"},
        };

        var result = LinesVerifier.Verify(testCase, PumpingDataRange.Daily, logger).ToList();

        result.ShouldBe(testCase);
        CheckLoggerCalls(0);
    }

    [TestCase("2022120")]
    [TestCase("202212010")]
    [TestCase("")]
    [TestCase("abcdqwer")]
    [TestCase("2022120X")]
    public void Verify_Daily_IncorectDate__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", value, "0.2980", "0.3280", "0.2980", "0.3000", "24000"},
        };

        CheckIncorrectDaily(testCase);
    }

    [TestCaseSource(nameof(ohlcIncorrectValues))]
    public void Verify_Daily_IncorectOpen__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", value, "0.3280", "0.2980", "0.3000", "24000"},
        };

        CheckIncorrectDaily(testCase);
    }

    [TestCaseSource(nameof(ohlcIncorrectValues))]
    public void Verify_Daily_IncorectHigh__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", "0.2980", value, "0.2980", "0.3000", "24000"},
        };

        CheckIncorrectDaily(testCase);
    }

    [TestCaseSource(nameof(ohlcIncorrectValues))]
    public void Verify_Daily_IncorectLow__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", "0.2980", "0.3280", value, "0.3000", "24000"},
        };

        CheckIncorrectDaily(testCase);
    }

    [TestCaseSource(nameof(ohlcIncorrectValues))]
    public void Verify_Daily_IncorectClose__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", "0.2980", "0.3280", "0.2980", value, "24000"},
        };

        CheckIncorrectDaily(testCase);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("abcd")]
    [TestCase("2400X")]
    [TestCase("24000.0")]
    public void Verify_Daily_IncorectVolumee__SkipsIncorrectLine(string value)
    {
        List<string[]> testCase = new()
        {
            new string[] {"ADATEX", "20221201", "0.2980", "0.3280", "0.2980", "0.3000", value},
        };

        CheckIncorrectDaily(testCase);
    }

    [Test]
    public void Verify_Daily_EmptyList__ReturnsEmpty()
    {
        var result = LinesVerifier.Verify(new List<string[]>(), PumpingDataRange.Daily, logger).ToList();

        result.ShouldBeEmpty();
        CheckLoggerCalls(0);
    }

    [Test]
    public void Verify_Ticks__Throws()
    {
        Should.Throw<ArgumentException>(() => LinesVerifier.Verify(new List<string[]>(), PumpingDataRange.Tick, logger).ToList());
    }

    private void CheckIncorrectDaily(List<string[]> testCase)
    {
        var result = LinesVerifier.Verify(testCase, PumpingDataRange.Daily, logger).ToList();

        result.ShouldBeEmpty();
        CheckLoggerCalls(1);
    }

    private void CheckLoggerCalls(int expectedCalls) =>
        logger.ReceivedWithAnyArgs(expectedCalls).Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception?>(), Arg.Any<Func<object, Exception?, string>>());
}
