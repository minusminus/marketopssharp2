using AutoFixture;

namespace MarketOps.Tests.Autofixture;

/// <summary>
/// Factory of Autofixture fixtures.
/// </summary>
internal static class FixtureFactory
{
    public static IFixture Get() =>
        new Fixture();
}
