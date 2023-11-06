using MarketOps.DataPump.CliCommands;
using MarketOps.DataPump.Common;
using MarketOps.DataPump.SetUp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

namespace MarketOps.DataPump;

internal class Program
{
    static async Task Main(string[] args)
    {
        ExecutionOptions executionOptions = new();

        await DefineCommand.Define(executionOptions)
            .InvokeAsync(args);

        if (!executionOptions.ParsedCorrectly) return;

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.ConfigureAppConfig())
            .ConfigureLogging(loggingBuilder => loggingBuilder.ConfigureLogging())
            .ConfigureServices((context, services) => services.RegisterServices(context.Configuration, executionOptions.SimulateStore))
            .Build();

        var mainTask = host.RunAsync();
        
        var executor = host.Services.GetRequiredService<IDataPumpExecutor>();
        executor.Execute(executionOptions.StockTypes);

        await mainTask;
    }
}
