using MarketOps.Scanner.Common;

namespace MarketOps.Tests.Common;

[TestFixture]
internal class ScanResultTests
{
    private readonly Dictionary<string, object> _additionalData = [];

    [Test]
    public void Initialize__AleStatesUnspecified_AndNullAdditionalData()
    {
        var result = ScanResult.Initialize(5);

        result.All(x => (x.Signal == ScanResultSignal.Uspecified) && (x.AdditionalData is null)).ShouldBeTrue();
    }

    [Test]
    public void Signaled__StateIsSignaled_AdditinalDataSet()
    {
        var result = ScanResult.Signaled(_additionalData);

        result.Signal.ShouldBe(ScanResultSignal.Signal);
        result.AdditionalData.ShouldBe(_additionalData);
    }

    [Test]
    public void NotSignaled__StateIsNotSignaled_AdditinalDataSet()
    {
        var result = ScanResult.NotSignaled(_additionalData);

        result.Signal.ShouldBe(ScanResultSignal.NoSignal);
        result.AdditionalData.ShouldBe(_additionalData);
    }
}
