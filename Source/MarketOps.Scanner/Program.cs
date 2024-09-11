using MarketOps.Scanner.CliCommands;
using MarketOps.Scanner.SetUp;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace MarketOps.Scanner;

internal class Program
{
    public static async Task Main(string[] args)
    {
        ExecutionOptions executionOptions = new();

        await DefineCommand.Define(executionOptions)
            .InvokeAsync(args);

        if (!executionOptions.ParsedCorrectly) return;

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.ConfigureAppConfig())
            .ConfigureLogging(loggingBuilder => loggingBuilder.ConfigureLogging())
            .ConfigureServices((context, services) => services.RegisterServices(context.Configuration, executionOptions))
            .Build();

        await host.RunAsync();
    }
}
