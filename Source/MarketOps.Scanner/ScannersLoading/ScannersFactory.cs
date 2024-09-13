using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.Common;
using System.Reflection;

namespace MarketOps.Scanner.ScannersLoading;

/// <summary>
/// Factory of scanners.
/// 
/// Searches for specified class in all MarketOps.Scanner.*.dll files.
/// </summary>
internal class ScannersFactory : IScannersFactory
{
    private const string AssemblyNamePrefix = "MarketOps.Scanner.Scanners";

    public IScanner? GetScanner(string scannerName)
    {
        var assemblies = PreloadAssemblies();
        foreach (var assembly in assemblies)
        {
            var scanner = FindScanner(assembly, scannerName);
            if (scanner is not null)
                return scanner;
        }
        return default;
    }

    private static Assembly[] PreloadAssemblies()
    {
        //var classLibraries = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, AssemblyNamePrefix + "*.dll");
        var classLibraries = Directory.GetFiles(Consts.ExecutingLocation, AssemblyNamePrefix + "*.dll");
        
        return classLibraries
            .Select(fileName => AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(fileName)))
            .ToArray();
    }

    private static IScanner? FindScanner(Assembly assembly, string scannerName) => 
        assembly
            .GetTypes()
            .Where(type => type.Name == scannerName)
            .Where(type => type.IsClass && type.IsAssignableTo(typeof(IScanner)))
            .Select(type => (IScanner)Activator.CreateInstance(type)!)
            .FirstOrDefault();
}
