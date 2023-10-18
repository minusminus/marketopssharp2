using MarketOps.DataPump.SetUp;
using Microsoft.Extensions.Hosting;

namespace MarketOps.DataPump;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.ConfigureAppConfig())
            .ConfigureServices((context, services) => services.RegisterServices(context.Configuration))
            .Build();

        host.Run();
    }
}
