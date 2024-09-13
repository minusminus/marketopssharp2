using MarketOps.Scanner.Scanners.Calculators;

namespace MarketOps.Tests.Scanner.Scanners.Calculators;

[TestFixture]
public class HLChannelTests
{
    [TestCase(new[] { 5f, 6f, 7f, 8f, 9f }, new[] { 1f, 2f, 3f, 4f, 5f }, 3, new[] { 7f, 8f, 9f }, new[] { 1f, 2f, 3f })]
    [TestCase(new[] { 9f, 8f, 7f, 6f, 5f }, new[] { 5f, 4f, 3f, 2f, 1f }, 3, new[] { 9f, 8f, 7f }, new[] { 3f, 2f, 1f })]
    [TestCase(new[] { 5f, 6f, 7f }, new[] { 1f, 2f, 3f }, 3, new[] { 7f }, new[] { 1f })]
    [TestCase(new[] { 9f, 8f, 7f }, new[] { 5f, 4f, 3f }, 3, new[] { 9f }, new[] { 3f })]
    [TestCase(new[] { 5f, 5f, 5f, 5f, 5f }, new[] { 1f, 1f, 1f, 1f, 1f }, 3, new[] { 5f, 5f, 5f }, new[] { 1f, 1f, 1f })]
    public void Calculate_CorrectInput__CalculatesCorrectly(float[] highs, float[] lows, int period, float[] expectedH, float[] expectedL)
    {
        HLChannelData res = HLChannel.Calculate(highs, lows, period);
        res.H.ShouldBe(expectedH);
        res.L.ShouldBe(expectedL);
    }

    [Test]
    public void Calculate_IncorrectPeriod__ReturnsEmptyLists([Range(-2, 0)] int period)
    {
        TestForEmptyList([8f, 9f, 10f], [1f, 2f, 3f], period);
    }

    [TestCase(new[] { 5f, 6f, 7f, 8f, 9f }, new[] { 1f, 2f, 3f, 4f }, 3)]
    [TestCase(new[] { 5f, 6f, 7f, 8f }, new[] { 1f, 2f, 3f, 4f, 5f }, 3)]
    public void Calculate_InputLengthsDiffers__ReturnsEmptyLists(float[] highs, float[] lows, int period)
    {
        TestForEmptyList(highs, lows, period);
    }

    [Test]
    public void Calculate_TooShortInput__ReturnsEmptyLists([Range(0, 4)] int inputLength)
    {
        TestForEmptyList(
            Enumerable.Range(0, inputLength).Select(i => 100f + i).ToArray(),
            Enumerable.Range(0, inputLength).Select(i => (float)i).ToArray(),
            5);
    }

    private void TestForEmptyList(float[] highs, float[] lows, int period)
    {
        HLChannelData res = HLChannel.Calculate(highs, lows, period);
        res.H.Length.ShouldBe(0);
        res.L.Length.ShouldBe(0);
    }
}
