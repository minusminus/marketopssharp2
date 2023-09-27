using System.Reflection;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Constants used in DataPump.
/// </summary>
public static class Consts
{
    public static string ExecutingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
}
