using System.CommandLine;
using MarketOps.Scanner.Abstractions;

namespace MarketOps.Scanner.CliCommands;

/// <summary>
/// Command line command definition.
/// Properly parsed arguments are rewritten to ScanningOptions to be used in processing.
/// </summary>
internal static class DefineCommand
{
    public static RootCommand Define(ScanningOptions scanningOptions)
    {
        var rootCommand = new RootCommand($"{nameof(MarketOps.Scanner)} scanning stocks data in search for signals.");
        var argScannerName = rootCommand.CreateArgumentScannerName();
        var argStockNamesFilePath = rootCommand.CreateArgumentStockNamesFilePath();
        var argResultsPath = rootCommand.CreateArgumentResultsPath();
        var optNumberOfSignalsPerStock = rootCommand.CreateOptionNumberOfSignalsPerStock();
        var optScannerParameters = rootCommand.CreateOptionScannerParameters();

        rootCommand.SetHandler((scannerName, stockNamesFilePath, resultsPath, numberOfSignalsPerStock, scannerParameters) =>
        {
            scanningOptions.ParsedCorrectly = true;
            scanningOptions.ScannerName = scannerName;
            scanningOptions.StockNamesFilePath = stockNamesFilePath;
            scanningOptions.ResultsPath = resultsPath;
            scanningOptions.NumberOfSignalsPerStock = numberOfSignalsPerStock;
            scanningOptions.ScannerParametersJson = scannerParameters;
        }, argScannerName, argStockNamesFilePath, argResultsPath, optNumberOfSignalsPerStock, optScannerParameters);

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
            description: "Path to the file containing stock names to scan.")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddArgument(argument);
        return argument;
    }

    private static Argument<string> CreateArgumentResultsPath(this Command command)
    {
        var argument = new Argument<string>("resultsPath",
            description: "Path where scanning results will be stored.",
            getDefaultValue: () => string.Empty)
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
            getDefaultValue: () => 1)
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddOption(option);
        return option;
    }

    private static Option<string> CreateOptionScannerParameters(this Command command)
    {
        var option = new Option<string>("--scannerParameters",
            description: "Scanner parameters formatted as JSON in string with '\"' escaped with '\\'",
            getDefaultValue: () => string.Empty)
        {
            Arity = ArgumentArity.ExactlyOne
        };

        command.AddOption(option);
        return option;
    }
}
