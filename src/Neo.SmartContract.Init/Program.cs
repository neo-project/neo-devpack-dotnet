using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Spectre.Console;
using Neo.SmartContract.Init.Commands;
using Neo.SmartContract.Init.Services;

namespace Neo.SmartContract.Init;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("NEO Smart Contract Project Initializer");

        // Add commands
        rootCommand.AddCommand(new NewCommand());
        rootCommand.AddCommand(new ListCommand());
        rootCommand.AddCommand(new AddCommand());

        // Add global options
        var verboseOption = new Option<bool>(
            new[] { "--verbose", "-v" },
            "Enable verbose output");
        rootCommand.AddGlobalOption(verboseOption);

        // Display banner when no args
        if (args.Length == 0)
        {
            DisplayBanner();
            await rootCommand.InvokeAsync(new[] { "--help" });
            return 0;
        }

        return await rootCommand.InvokeAsync(args);
    }

    private static void DisplayBanner()
    {
        AnsiConsole.Write(new FigletText("NEO Init")
            .Color(Color.Green));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]NEO Smart Contract Project Initializer[/]");
        AnsiConsole.MarkupLine("[dim]Create and manage NEO smart contract projects with ease[/]");
        AnsiConsole.WriteLine();
    }
}
