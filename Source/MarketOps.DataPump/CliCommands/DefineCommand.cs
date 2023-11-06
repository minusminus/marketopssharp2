using MarketOps.Types;
using System.CommandLine.Parsing;
using System.CommandLine;

namespace MarketOps.DataPump.CliCommands;

/// <summary>
/// Command line command definition.
/// Properly parsed arguments are rewritten to ExecutionOptions to be used in processing.
/// </summary>
internal static class DefineCommand
{
    public static RootCommand Define(ExecutionOptions executionOptions)
    {
        var rootCommand = new RootCommand($"{nameof(MarketOps.DataPump)} pumping stocks data.");
        var argStockTypes = rootCommand.CreateArgumentStockTypes();
        var optSimulateStore = rootCommand.CreateOptionSimulateStore();

        rootCommand.SetHandler((stockTypes, simulateStore) =>
        {
            executionOptions.ParsedCorrectly = true;
            executionOptions.StockTypes = stockTypes;
            executionOptions.SimulateStore = simulateStore;
        }, argStockTypes, optSimulateStore);

        return rootCommand;
    }

    private static Argument<StockType[]> CreateArgumentStockTypes(this Command command)
    {
        var argument = new Argument<StockType[]>("stockTypes",
            description: "List of comma separated stock types to process.\nAll active stocks of selected types are processed.",
            parse: argumentResult => ParseStockTypesToken(argumentResult))
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddArgument(argument);
        return argument;

        static StockType[] ParseStockTypesToken(ArgumentResult argumentResult)
        {
            var splittedItems = argumentResult.Tokens.Single().Value.Split(',');
            var output = new StockType[splittedItems.Length];
            for (int i = 0; i < splittedItems.Length; i++)
            {
                if (!Enum.TryParse<StockType>(splittedItems[i], true, out var parsedValue))
                {
                    argumentResult.ErrorMessage = $"Unknown StockType: {splittedItems[i]}";
                    return Array.Empty<StockType>();
                }
                output[i] = parsedValue;
            }
            return output;
        }
    }

    private static Option<bool> CreateOptionSimulateStore(this Command command)
    {
        var option = new Option<bool>("--simulateStore", 
            description: "Simulate data storing.\nAll provided data will be iterated, but not stored. Simulating allows detection of data line errors, or missing data files.");

        command.AddOption(option);
        return option;
    }
}
