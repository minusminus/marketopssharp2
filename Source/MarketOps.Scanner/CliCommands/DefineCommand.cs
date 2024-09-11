using System.CommandLine.Parsing;
using System.CommandLine;

namespace MarketOps.Scanner.CliCommands;

/// <summary>
/// Command line command definition.
/// Properly parsed arguments are rewritten to ExecutionOptions to be used in processing.
/// </summary>
internal static class DefineCommand
{
    public static RootCommand Define(ExecutionOptions executionOptions)
    {
        var rootCommand = new RootCommand($"{nameof(MarketOps.Scanner)} scanning stocks data in search for signals.");
        var argScannerName = rootCommand.CreateArgumentScannerName();
        var argStockNamesFilePath = rootCommand.CreateArgumentStockNamesFilePath();
        var optNumberOfSignalsPerStock = rootCommand.CreateOptionNumberOfSignalsPerStock();

        rootCommand.SetHandler((scannerName, stockNamesFilePath, numberOfSignalsPerStock) =>
        {
            executionOptions.ParsedCorrectly = true;
            executionOptions.ScannerName = scannerName;
            executionOptions.StockNamesFilePath = stockNamesFilePath;
            executionOptions.NumberOfSignalsPerStock = numberOfSignalsPerStock;
        }, argScannerName, argStockNamesFilePath, optNumberOfSignalsPerStock);

        return rootCommand;
    }

    private static Argument<string> CreateArgumentScannerName(this Command command)
    {
        var argument = new Argument<string>("scannerName",
            description: "Scanner name")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddArgument(argument);
        return argument;
    }

    private static Argument<string> CreateArgumentStockNamesFilePath(this Command command)
    {
        var argument = new Argument<string>("stockNamesFilePath",
            description: "Path to file with stock names to scan.")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddArgument(argument);
        return argument;
    }

    private static Option<int> CreateOptionNumberOfSignalsPerStock(this Command command)
    {
        var option = new Option<int>("--numberOfSignalsPerStock",
            description: "Number of signals generated for each stock",
            getDefaultValue: DefaultNumberOfSignalsPerStock)
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddOption(option);
        return option;

        int DefaultNumberOfSignalsPerStock() => 1;
    }
}
