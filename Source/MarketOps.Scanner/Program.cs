using MarketOps.Scanner.Abstractions;
using MarketOps.Scanner.CliCommands;
using MarketOps.Scanner.SetUp;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace MarketOps.Scanner;

internal class Program
{
    public static async Task Main(string[] args)
    {
        ScanningOptions scanningOptions = new();

        await DefineCommand.Define(scanningOptions)
            .InvokeAsync(args);

        if (!scanningOptions.ParsedCorrectly) return;

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.ConfigureAppConfig())
            .ConfigureLogging(loggingBuilder => loggingBuilder.ConfigureLogging())
            .ConfigureServices((context, services) => services.RegisterServices(context.Configuration, scanningOptions))
            .Build();

        await host.RunAsync();
    }
}
