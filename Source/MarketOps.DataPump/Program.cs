using MarketOps.DataPump.Common;
using MarketOps.DataPump.SetUp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarketOps.DataPump;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.ConfigureAppConfig())
            .ConfigureLogging(loggingBuilder => loggingBuilder.ConfigureLogging())
            .ConfigureServices((context, services) => services.RegisterServices(context.Configuration))
            .Build();

        var mainTask = host.RunAsync();

        var executor = host.Services.GetRequiredService<IDataPumpExecutor>();
        executor.Execute(Types.StockType.Index);

        await mainTask;
    }
}
