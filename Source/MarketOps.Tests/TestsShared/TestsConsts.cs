using System.Reflection;

namespace MarketOps.Tests.TestsShared;

internal static class TestsConsts
{
    public static string ExecutingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
}
