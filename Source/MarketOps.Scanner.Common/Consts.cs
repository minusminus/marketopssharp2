using System.Reflection;

namespace MarketOps.Scanner.Common;

/// <summary>
/// Constants used in Scanner.
/// </summary>
public static class Consts
{
    public static string ExecutingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public const string ConfigFileName = "appconfig.json";
}
