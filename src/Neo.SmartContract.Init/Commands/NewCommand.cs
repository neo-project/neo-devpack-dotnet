using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Spectre.Console;
using Neo.SmartContract.Init.Services;
using Neo.SmartContract.Init.Models;

namespace Neo.SmartContract.Init.Commands;

public class NewCommand : Command
{
    public NewCommand() : base("new", "Create a new NEO smart contract project")
    {
        var nameArgument = new Argument<string>(
            "name",
            "The name of the project");

        var templateOption = new Option<string>(
            new[] { "--template", "-t" },
            getDefaultValue: () => "basic",
            "The template to use (basic, nep17, nep11, oracle, multisig, dao)");

        var outputOption = new Option<string>(
            new[] { "--output", "-o" },
            getDefaultValue: () => ".",
            "The output directory");

        var forceOption = new Option<bool>(
            new[] { "--force", "-f" },
            "Overwrite existing files");

        var interactiveOption = new Option<bool>(
            new[] { "--interactive", "-i" },
            "Use interactive mode");

        AddArgument(nameArgument);
        AddOption(templateOption);
        AddOption(outputOption);
        AddOption(forceOption);
        AddOption(interactiveOption);

        this.SetHandler(async (InvocationContext context) =>
        {
            var name = context.ParseResult.GetValueForArgument(nameArgument);
            var template = context.ParseResult.GetValueForOption(templateOption);
            var output = context.ParseResult.GetValueForOption(outputOption);
            var force = context.ParseResult.GetValueForOption(forceOption);
            var interactive = context.ParseResult.GetValueForOption(interactiveOption);

            await HandleNewCommand(name, template!, output!, force, interactive);
        });
    }

    private async Task HandleNewCommand(string name, string template, string output, bool force, bool interactive)
    {
        var projectService = new ProjectService();
        var templateService = new TemplateService();

        ProjectConfig config;

        if (interactive || string.IsNullOrEmpty(name))
        {
            config = await RunInteractiveMode(templateService);
        }
        else
        {
            config = new ProjectConfig
            {
                Name = name,
                Template = template,
                OutputPath = output,
                Author = "Your Name",
                Email = "your.email@example.com",
                Description = $"NEO smart contract project: {name}"
            };
        }

        // Create project with progress
        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Creating project...[/]");
                
                await projectService.CreateProject(config, force, (progress, message) =>
                {
                    task.Value = progress;
                    task.Description = message;
                });
                
                task.Value = 100;
                task.Description = "[green]Project created successfully![/]";
            });

        // Display success message
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold green]✓[/] Project [bold]{config.Name}[/] created successfully!");
        AnsiConsole.WriteLine();
        
        // Display next steps
        var panel = new Panel(new Markup($@"[bold]Next steps:[/]

1. Navigate to your project:
   [blue]cd {Path.Combine(config.OutputPath, config.Name)}[/]

2. Build your contract:
   [blue]dotnet build[/]

3. Run tests:
   [blue]dotnet test[/]

4. Compile to NEF:
   [blue]dotnet run --project ../../src/Neo.Compiler.CSharp -- {config.Name}.csproj[/]

For more information, see the [link]https://github.com/neo-project/neo-devpack-dotnet/blob/master/docs/getting-started.md[/]"))
            .Header("[bold]Get Started[/]")
            .BorderColor(Color.Green);
        
        AnsiConsole.Write(panel);
    }

    private async Task<ProjectConfig> RunInteractiveMode(TemplateService templateService)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("NEO Init").Color(Color.Green));
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Let's create a new NEO smart contract project![/]");
        AnsiConsole.WriteLine();

        var config = new ProjectConfig();

        // Project name
        config.Name = AnsiConsole.Ask<string>("[bold]Project name:[/]");

        // Template selection
        var templates = templateService.GetAvailableTemplates();
        var templateChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Select a template:[/]")
                .AddChoices(templates.Select(t => $"{t.Id} - {t.Description}"))
                .HighlightStyle(new Style(Color.Green)));
        
        config.Template = templateChoice.Split(" - ")[0];

        // Author information
        config.Author = AnsiConsole.Ask("[bold]Author name:[/]", "Your Name");
        config.Email = AnsiConsole.Ask("[bold]Author email:[/]", $"{config.Author}@example.com");
        config.Description = AnsiConsole.Ask("[bold]Project description:[/]", $"NEO smart contract project: {config.Name}");

        // Output directory
        config.OutputPath = AnsiConsole.Ask("[bold]Output directory:[/]", ".");

        // Additional features
        var features = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("[bold]Select additional features:[/]")
                .NotRequired()
                .AddChoices(new[]
                {
                    "Unit Tests",
                    "GitHub Actions CI/CD",
                    "Docker Support",
                    "VS Code Configuration",
                    "Security Analyzer",
                    "Gas Optimization"
                })
                .HighlightStyle(new Style(Color.Green)));

        config.Features = features.ToList();

        return config;
    }
}