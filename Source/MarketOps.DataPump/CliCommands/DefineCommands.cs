using MarketOps.Types;
using System.CommandLine;

namespace MarketOps.DataPump.CliCommands;

internal static class DefineCommands
{
    public static RootCommand Define()
    {
        var rootCommand = new RootCommand($"{nameof(MarketOps.DataPump)} pumping stocks data.");
        var argStockTypes = rootCommand.CreateArgumentStockTypes();
        var optSimulateStore = rootCommand.CreateOptionSimulateStore();

        rootCommand.SetHandler((stockTypes, simulateStore) =>
        {
            foreach (var item in stockTypes)
            {
                Console.WriteLine(item);
            }
        }, argStockTypes, optSimulateStore);

        return rootCommand;
    }

    private static Argument<StockType[]> CreateArgumentStockTypes(this Command command)
    {
        const string argumentName = "stockTypes";

        var argument = new Argument<StockType[]>(argumentName, 
            description: "List of comma separated stock types to process.\nAll active stocks of selected types are processed.",
            parse: result =>
            {
                var items = result.Tokens.Single().Value.Split(',');
                var output = new StockType[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    if (!Enum.TryParse<StockType>(items[i], true, out var parsed))
                    {
                        result.ErrorMessage = $"Unknown StockType: {items[i]}";
                        return Array.Empty<StockType>();
                    }
                    output[i] = parsed;
                }
                return output;
            });
        argument.Arity = ArgumentArity.ExactlyOne;

        argument.AddValidator(argumentResult =>
        {
            var value = argumentResult.GetValueOrDefault<StockType[]>();
            if ((value is null) || (value.Length == 0))
                argumentResult.ErrorMessage += $"{argumentName} is empty";
        });

        command.AddArgument(argument);
        return argument;
    }

    private static Option<bool> CreateOptionSimulateStore(this Command command)
    {
        var argument = new Option<bool>("--simulateStore", "Simulate data storing.\nAll provided data will be iterated, but not stored. Simulating allows detection of data line errors, or missing data files.");

        command.AddOption(argument);
        return argument;
    }
}
