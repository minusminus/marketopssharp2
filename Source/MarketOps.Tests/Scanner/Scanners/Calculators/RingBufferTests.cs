using MarketOps.Scanner.Scanners.Calculators;

namespace MarketOps.Tests.Scanner.Scanners.Calculators;

[TestFixture]
public class RingBufferTests
{
    private static readonly int TestLength = 10;
    private RingBuffer<int> _testObj;

    [SetUp]
    public void SetUp()
    {
        _testObj = new RingBuffer<int>(TestLength);
    }

    [Test]
    public void Create__ProperlyInitialized()
    {
        _testObj.Length.ShouldBe(TestLength);
        _testObj.Filled.ShouldBeFalse();
    }

    [Test]
    public void Add_OneElement__Adds()
    {
        _testObj.Add(1);
        _testObj.Length.ShouldBe(TestLength);
        _testObj.Filled.ShouldBeFalse();
    }

    [Test]
    public void Add_ToBufferLength__Adds()
    {
        for (int i = 0; i < TestLength; i++)
            _testObj.Add(i);
        _testObj.Length.ShouldBe(TestLength);
        _testObj.Filled.ShouldBeTrue();
        for (int i = 0; i < TestLength; i++)
            _testObj.ElementAt(i).ShouldBe(i);
    }

    [Test]
    public void Add_OverBufferLength__Adds()
    {
        const int excess = 3;
        for (int i = 0; i < TestLength + excess; i++)
            _testObj.Add(i);
        _testObj.Length.ShouldBe(TestLength);
        _testObj.Filled.ShouldBeTrue();
        for (int i = 0; i < TestLength; i++)
            _testObj.ElementAt(i).ShouldBe(i + excess);
    }
}
